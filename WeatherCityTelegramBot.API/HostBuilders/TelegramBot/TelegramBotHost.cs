using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using WeatherCityTelegramBot.BLL.EnvironmentVariables;
using WeatherCityTelegramBot.BLL.Services.Contracts;
using WeatherCityTelegramBot.DAL.DTOs.WeatherDTOs;
using WeatherCityTelegramBot.DAL.Entities;
using WeatherCityTelegramBot.DAL.Persistence.Contracts;

namespace WeatherCityTelegramBot.API.HostBuilders.TelegramBot
{
    public class TelegramBotHost
    {
        public TelegramBotClient telegramBotClient;
        private readonly ILogger<TelegramBotHost> logger;
        IServiceScope serviceScope;
        private readonly IWeatherApiService weatherApiService;

        public TelegramBotHost(IConfiguration configuration, ILogger<TelegramBotHost> logger, IServiceScopeFactory serviceScopeFactory, IWeatherApiService weatherApiService)
        {
            telegramBotClient = new TelegramBotClient(configuration.GetValue<string>("TelegramBotToken") ?? "");
            this.logger = logger;
            serviceScope = serviceScopeFactory.CreateScope();
            this.weatherApiService = weatherApiService;
        }

        public void Start()
        {
            telegramBotClient.StartReceiving(UpdateHandler, ErrorHandler);
        }

        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            logger.LogError($"ERROR: {exception.Message}");
            await Task.CompletedTask;
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message?.Text == "/start")
            {
                logger.LogInformation($"Received /start message from: {update.Message.Chat.Username}");

                var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var userFromDB = await unitOfWork.UserRepository.GetAsync((int)update.Message.Chat.Id);

                if (userFromDB == null)
                {
                    var user = new DAL.Entities.User
                    {
                        Id = (int)update.Message.Chat.Id,
                        StartDate = DateTime.UtcNow,
                    };

                    await unitOfWork.UserRepository.AddAsync(user);
                }

                await client.SendMessage(update.Message.Chat.Id, "List of commands:\n" +
                        "   /weather {city} - check the current weather in any city\n");
            }
            else if (update.Message?.Text!.StartsWith("/weather") ?? false)
            {
                string input = update.Message?.Text!;
                string city = input.Length > "/weather".Length ? input.Substring("/weather".Length).Trim() : "";
                if (city.IsNullOrEmpty()) 
                {
                    await client.SendMessage(update.Message!.Chat.Id, "Parameter {city} could not be null,\n" +
                        " this command should be used in this format \n" +
                        "   /weather {city} \n" +
                        "where inplace of {city}, write the city you want to check current weather.");
                }
                else
                {
                    WeatherDTO weatherDTO = await weatherApiService.GetWeatherByCity(city);

                    if (weatherDTO == null)
                    {
                        await client.SendMessage(update.Message!.Chat.Id, "Please, check if the city is correctly written.");
                    }
                    else
                    {
                        var weatherHistory = new WeatherHistory
                        {
                            City = city,
                            CreationDate = DateTime.UtcNow,
                            UserId = (int)update.Message!.Chat.Id
                        };

                        var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var userFromDB = await unitOfWork.WeatherHistoryRepository.AddAsync(weatherHistory);

                        await client.SendMessage(update.Message!.Chat.Id, "Current weather in your city:\n\n" +
                            $"   Latitude: {weatherDTO.Latitude}\n" +
                            $"   Longitude: {weatherDTO.Longitude}\n" +
                            $"   Resolved Address: {weatherDTO.ReasolvedAddress}\n" +
                            $"   Address: {weatherDTO.Address}\n" +
                            $"   Timezone: {weatherDTO.Timezone}\n" +
                            $"   Tzoffset: {weatherDTO.Tzoffset}\n" +
                            $"   Description: {weatherDTO.Description}\n\n" +
                            $"To check weather in other cities use command:\n" +
                            "   /weather {city}");
                    }
                }
            }
            else
            {
                await client.SendMessage(update.Message?.Chat.Id!, "List of commands:\n" +
                        "   /weather {city} - check the current weather in any city\n");
            }
            await Task.CompletedTask;
        }
    }
}

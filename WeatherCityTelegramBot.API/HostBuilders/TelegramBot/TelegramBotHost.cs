using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace WeatherCityTelegramBot.API.HostBuilders.TelegramBot
{
    public class TelegramBotHost
    {
        private TelegramBotClient telegramBotClient;
        private readonly ILogger<TelegramBotHost> logger;

        public TelegramBotHost(IConfiguration configuration, ILogger<TelegramBotHost> logger)
        {
            telegramBotClient = new TelegramBotClient(configuration.GetValue<string>("TelegramBotToken") ?? "");
            this.logger = logger;
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
            }
            await Task.CompletedTask;
        }
    }
}

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using WeatherCityTelegramBot.API.HostBuilders.TelegramBot;
using WeatherCityTelegramBot.BLL.Services;
using WeatherCityTelegramBot.BLL.Services.Contracts;
using WeatherCityTelegramBot.DAL.Persistence.Contracts;

namespace WeatherCityTelegramBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UserController> logger;
        private TelegramBotHost telegramBotHost;
        private IWeatherApiService WeatherApiService;

        public UserController(IUnitOfWork unitOfWork, ILogger<UserController> logger, TelegramBotHost tegramBotHost, IWeatherApiService weatherApiService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.telegramBotHost = tegramBotHost;
            WeatherApiService = weatherApiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await unitOfWork.UserRepository.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWithWeatherHistories()
        {
            try
            {
                var result = await unitOfWork.UserRepository.GetUserWithWeatherHistoriesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{Id:int}")]
        public async Task<IActionResult> GetWithWeatherHistoriesById(int Id)
        {
            try
            {
                var result = await unitOfWork.UserRepository.GetUserWithWeatherHistoriesByIdAsync(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendWeatherToAll()
        {
            try
            {
                var result = await unitOfWork.UserRepository.GetUserWithWeatherHistoriesAsync();

                foreach (var item in result)
                {
                    var weatherResult = await WeatherApiService.GetWeatherByCity(item.WeatherHistories.Last().City);

                    await telegramBotHost.SendWeatherToUser(item.Id, weatherResult);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
    }
}

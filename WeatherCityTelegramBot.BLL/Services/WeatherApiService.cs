using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherCityTelegramBot.BLL.EnvironmentVariables;
using WeatherCityTelegramBot.BLL.Services.Contracts;
using WeatherCityTelegramBot.DAL.DTOs.WeatherDTOs;

namespace WeatherCityTelegramBot.BLL.Services
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly IOptions<WeatherApiEnvironment> weatherApiOptions;
        private readonly HttpClient httpClient;
        private readonly ILogger<WeatherApiService> logger;

        public WeatherApiService(IOptions<WeatherApiEnvironment> weatherApiOptions, HttpClient httpClient, ILogger<WeatherApiService> logger)
        {
            this.weatherApiOptions = weatherApiOptions;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<WeatherDTO> GetWeatherByCity(string city)
        {
            try
            {
                string url = $"{weatherApiOptions.Value.Domain}/timeline/{city}?key={weatherApiOptions.Value.APIKey}";

                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                var weatherDto = new WeatherDTO
                {
                    Latitude = root.GetProperty("latitude").GetDouble(),
                    Longitude = root.GetProperty("longitude").GetDouble(),
                    ReasolvedAddress = root.GetProperty("resolvedAddress").GetString(),
                    Address = root.GetProperty("address").GetString(),
                    Timezone = root.GetProperty("timezone").GetString(),
                    Tzoffset = root.GetProperty("tzoffset").GetDouble(),
                    Description = root.GetProperty("description").GetString()
                };

                return weatherDto;
            }
            catch (Exception ex)
            {
                logger.LogError($"Some error occured while trying to get weather: {ex.Message}");
                return null;
            }
        }
    }
}

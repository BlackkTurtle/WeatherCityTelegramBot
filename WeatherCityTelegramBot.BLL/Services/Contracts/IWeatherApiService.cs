using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCityTelegramBot.DAL.DTOs.WeatherDTOs;

namespace WeatherCityTelegramBot.BLL.Services.Contracts
{
    public interface IWeatherApiService
    {
        Task<WeatherDTO> GetWeatherByCity(string city);
    }
}

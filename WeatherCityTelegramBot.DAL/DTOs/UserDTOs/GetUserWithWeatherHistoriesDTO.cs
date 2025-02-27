using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCityTelegramBot.DAL.Entities;

namespace WeatherCityTelegramBot.DAL.DTOs.UserDTOs
{
    public class GetUserWithWeatherHistoriesDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public List<WeatherHistory> WeatherHistories { get; set; }
    }
}

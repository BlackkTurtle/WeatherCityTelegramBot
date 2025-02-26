using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCityTelegramBot.DAL.Entities;

namespace WeatherCityTelegramBot.DAL.Repositories.Contracts
{
    public interface IWeatherHistoryRepository : IGenericRepository<WeatherHistory>
    {
    }
}

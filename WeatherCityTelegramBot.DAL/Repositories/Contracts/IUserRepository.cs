using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCityTelegramBot.DAL.DTOs.UserDTOs;
using WeatherCityTelegramBot.DAL.Entities;

namespace WeatherCityTelegramBot.DAL.Repositories.Contracts
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<GetUserWithWeatherHistoriesDTO>> GetUserWithWeatherHistoriesAsync();
        Task<GetUserWithWeatherHistoriesDTO> GetUserWithWeatherHistoriesByIdAsync(int id);
    }
}

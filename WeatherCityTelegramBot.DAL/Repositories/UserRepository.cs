using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCityTelegramBot.DAL.DTOs.UserDTOs;
using WeatherCityTelegramBot.DAL.Entities;
using WeatherCityTelegramBot.DAL.Repositories.Contracts;

namespace WeatherCityTelegramBot.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SqlConnection sqlConnection, IDbTransaction dbtransaction) : base(sqlConnection, dbtransaction, "Users", "Id")
        {
        }

        public async Task<IEnumerable<GetUserWithWeatherHistoriesDTO>> GetUserWithWeatherHistoriesAsync()
        {
            var sql = @"SELECT u.Id, u.StartDate, 
                      w.Id AS WeatherHistoryId, w.City, w.CreationDate, w.UserId
                      FROM Users u
                      LEFT JOIN WeatherHistories w ON u.Id = w.UserId";

            var userDictionary = new Dictionary<int, GetUserWithWeatherHistoriesDTO>();

            var result = await dbConnection.QueryAsync<GetUserWithWeatherHistoriesDTO, WeatherHistory, GetUserWithWeatherHistoriesDTO>(
                sql,
                (user, weatherHistory) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.WeatherHistories = new List<WeatherHistory>();
                        userDictionary.Add(user.Id, userEntry);
                    }

                    if (weatherHistory != null)
                    {
                        userEntry.WeatherHistories.Add(weatherHistory);
                    }

                    return userEntry;
                },
                splitOn: "WeatherHistoryId",
                transaction: _dbTransaction
            );

            return userDictionary.Values;
        }

        public async Task<GetUserWithWeatherHistoriesDTO> GetUserWithWeatherHistoriesByIdAsync(int id)
        {
            var sql = @"SELECT u.Id, u.StartDate, 
                      w.Id AS WeatherHistoryId, w.City, w.CreationDate, w.UserId
                      FROM Users u
                      LEFT JOIN WeatherHistories w ON u.Id = w.UserId
                      WHERE u.Id = @UserId";

            var userDictionary = new Dictionary<int, GetUserWithWeatherHistoriesDTO>();

            var result = await dbConnection.QueryAsync<GetUserWithWeatherHistoriesDTO, WeatherHistory, GetUserWithWeatherHistoriesDTO>(
                sql,
                (user, weatherHistory) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.WeatherHistories = new List<WeatherHistory>();
                        userDictionary.Add(user.Id, userEntry);
                    }

                    if (weatherHistory != null)
                    {
                        userEntry.WeatherHistories.Add(weatherHistory);
                    }

                    return userEntry;
                },
                new { UserId = id },
                splitOn: "WeatherHistoryId",
                transaction: _dbTransaction
            );

            return userDictionary.Values.FirstOrDefault();
        }
    }
}

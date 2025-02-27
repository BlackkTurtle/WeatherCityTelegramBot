using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCityTelegramBot.DAL.Entities;
using WeatherCityTelegramBot.DAL.Repositories.Contracts;

namespace WeatherCityTelegramBot.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(IDbConnection sqlConnection, IDbTransaction dbtransaction) : base(sqlConnection, dbtransaction, "Users", "Id")
        {
        }
    }
}

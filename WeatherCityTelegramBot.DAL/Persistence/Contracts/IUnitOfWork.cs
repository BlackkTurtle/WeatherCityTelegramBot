using System;
using WeatherCityTelegramBot.DAL.Repositories.Contracts;

namespace WeatherCityTelegramBot.DAL.Persistence.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IWeatherHistoryRepository WeatherHistoryRepository { get; }
        void Commit();
        void Dispose();
    }
}

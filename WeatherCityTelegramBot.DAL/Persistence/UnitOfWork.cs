using System;
using System.Data;
using WeatherCityTelegramBot.DAL.Persistence.Contracts;
using WeatherCityTelegramBot.DAL.Repositories.Contracts;

namespace WeatherCityTelegramBot.DAL.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public IUserRepository UserRepository { get; }
        public IWeatherHistoryRepository WeatherHistoryRepository { get; }

        readonly IDbTransaction _dbTransaction;

        public UnitOfWork(
            IDbTransaction dbTransaction,
            IUserRepository userRepository,
            IWeatherHistoryRepository weatherHistoryRepository)
        {
            _dbTransaction = dbTransaction;
            UserRepository = userRepository;
            WeatherHistoryRepository = weatherHistoryRepository;
        }

        public void Commit()
        {
            try
            {
                _dbTransaction.Commit();
            }
            catch (Exception ex)
            {
                _dbTransaction.Rollback();
                Console.WriteLine(ex.Message);
            }
        }
        public void Dispose()
        {
            _dbTransaction.Connection?.Close();
            _dbTransaction.Connection?.Dispose();
            _dbTransaction.Dispose();
        }
    }
}
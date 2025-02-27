using System.Reflection;
using System.Text;
using System.Data;
using Dapper;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WeatherCityTelegramBot.DAL.Repositories.Contracts;
using Microsoft.Data.SqlClient;

namespace WeatherCityTelegramBot.DAL.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        protected IDbConnection dbConnection;

        protected IDbTransaction _dbTransaction;

        private readonly string _tableName;
        private readonly string _idName;

        protected GenericRepository(IDbConnection dbConnection, IDbTransaction dbTransaction, string tableName, string idName)
        {
            _dbTransaction = dbTransaction;
            _tableName = tableName;
            _idName = idName;
            this.dbConnection = dbConnection;
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbConnection.QueryAsync<T>($"SELECT * FROM {_tableName}",
                transaction: _dbTransaction);
        }

        public async Task<T> GetAsync(int id)
        {
            var result = await dbConnection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE {_idName}=@Id",
                param: new { Id = id },
                transaction: _dbTransaction);
            return result;
        }

        public async Task DeleteAsync(int id)
        {
            await dbConnection.ExecuteAsync($"DELETE FROM {_tableName} WHERE {_idName}=@Id",
                param: new { Id = id },
                transaction: _dbTransaction);
        }

        public async Task<int> AddAsync(T t)
        {
            var insertQuery = GenerateInsertQuery();
            var newId = await dbConnection.ExecuteScalarAsync<int>(insertQuery,
                param: t,
                transaction: _dbTransaction);
            return newId;
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> list)
        {
            var inserted = 0;
            var query = GenerateInsertQuery();
            inserted += await dbConnection.ExecuteAsync(query,
                param: list);
            return inserted;
        }


        public async Task ReplaceAsync(T t)
        {
            var updateQuery = GenerateUpdateQuery();
            await dbConnection.ExecuteAsync(updateQuery,
                param: t,
                transaction: _dbTransaction);
        }

        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();
        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }

        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(property =>
            {
                if (!property.Equals($"{_idName}"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });
            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append($" WHERE {_idName}=@{_idName}");
            return updateQuery.ToString();
        }

        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");
            insertQuery.Append("(");
            var properties = GenerateListOfProperties(GetProperties);

            if (_tableName != "Users")
            {
                properties.Remove($"{_idName}");
            }
            //
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");
            insertQuery.Append("; SELECT SCOPE_IDENTITY()");
            return insertQuery.ToString();
        }
    }
}

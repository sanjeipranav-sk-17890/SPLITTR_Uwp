using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace SPLITTR_Uwp.Core.Services.SqliteConnection
{
    public class SqlDataServices : ISqlDataServices
    {
        private readonly SQLiteAsyncConnection _connection;

        public SqlDataServices()
        {
            var connectionString = GetConnectionString();
            _connection = new SQLiteAsyncConnection(connectionString);
            
        }

        public async Task CreateTable<T>() where T : new()
        {
           await _connection.CreateTableAsync<T>().ConfigureAwait(false);
        }

        public AsyncTableQuery<T> FetchTable<T>() where T : new()
        {
           
                return _connection.Table<T>();
            
        }

        public Task<int> InsertObj<T>(T obj)
        {
            return _connection.InsertAsync(obj, typeof(T));
        }

        public Task<int> InsertObjects<T>(IEnumerable<T> objs)
        {
            return _connection.InsertAllAsync(objs, typeof(T));
        }

        public Task<int> UpdateObj<T>(T obj)
        {
            
            return _connection.UpdateAsync(obj, typeof(T));
        }

        private string GetConnectionString()
        {
            try
            {
                var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                return connectionString;

            }
            catch (ConfigurationException ex)
            {
                throw new Exception("Failed to Fetch Db Connectionstring From App.config", ex);
            }

        }
    }
}

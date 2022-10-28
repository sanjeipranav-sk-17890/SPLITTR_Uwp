using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace SPLITTR_Uwp.Core.Services.SqliteConnection
{
    public interface ISqlDataServices
    {
        Task CreateTable<T>() where T :new ();
        AsyncTableQuery<T> FetchTable<T>() where T : new();
        Task<int> InsertObj<T>(T obj);
        Task<int> InsertObjects<T>(IEnumerable<T> objs);
        Task<int> UpdateObj<T>(T obj);
    }
}
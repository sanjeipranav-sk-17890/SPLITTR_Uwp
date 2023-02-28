using System.Net.Http;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Core.Adapters.NetAdapter;

public interface INetAdapter
{
    Task<HttpResponseMessage> GetAsync(string baseUri,string uri, string accessToken = null, bool forceRefresh = false);

    //Task<bool> PostAsync<T>(string uri, T item);
    //Task<bool> PostAsJsonAsync<T>(string uri, T item);
    //Task<bool> PutAsync<T>(string uri, T item);
    //Task<bool> PutAsJsonAsync<T>(string uri, T item);
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.SplittrExceptions;

namespace SPLITTR_Uwp.Core.Adapters.NetAdapter;

public class HttpDataService : INetAdapter
{
    private readonly Dictionary<string, HttpResponseMessage> _responseCache;
    private readonly HttpClient _client;

    public HttpDataService()
    {
        _client = new HttpClient();

        _responseCache = new Dictionary<string, HttpResponseMessage>();
    }

    public async Task<HttpResponseMessage> GetAsync(string baseUri,string uri, string accessToken = null, bool forceRefresh = true)
    {
        ThrowIfNetworkNotAvailable();

         HttpResponseMessage result;

        _client.BaseAddress ??= new Uri(baseUri);

        // The responseCache is a simple store of past responses to avoid unnecessary requests for the same resource.
        if (forceRefresh || !_responseCache.ContainsKey(uri))
        {
            AddAuthorizationHeader(accessToken);
            result = await _client.GetAsync(uri).ConfigureAwait(false);

            if (_responseCache.ContainsKey(uri))
            {
                _responseCache[uri] = result;
            }
            else
            {
                _responseCache.Add(uri, result);
            }
        }
        else
        {
            result = _responseCache[uri];
        }

        return result;
    }
    private static void ThrowIfNetworkNotAvailable()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
        {
            throw new NoInterNetException();
        }
    }

    //public async Task<bool> PostAsync<T>(string uri, T item)
    //{
    //    if (item == null)
    //    {
    //        return false;
    //    }

    //    var serializedItem = JsonConvert.SerializeObject(item);
    //    var buffer = Encoding.UTF8.GetBytes(serializedItem);
    //    var byteContent = new ByteArrayContent(buffer);

    //    var response = await _client.PostAsync(uri, byteContent).ConfigureAwait(false);

    //    return response.IsSuccessStatusCode;
    //}

    //public async Task<bool> PostAsJsonAsync<T>(string uri, T item)
    //{
    //    if (item == null)
    //    {
    //        return false;
    //    }

    //    var serializedItem = JsonConvert.SerializeObject(item);

    //    var response = await _client.PostAsync(uri, new StringContent(serializedItem, Encoding.UTF8, "application/json")).ConfigureAwait(false);

    //    return response.IsSuccessStatusCode;
    //}

    //public async Task<bool> PutAsync<T>(string uri, T item)
    //{
    //    if (item == null)
    //    {
    //        return false;
    //    }

    //    var serializedItem = JsonConvert.SerializeObject(item);
    //    var buffer = Encoding.UTF8.GetBytes(serializedItem);
    //    var byteContent = new ByteArrayContent(buffer);

    //    var response = await _client.PutAsync(uri, byteContent).ConfigureAwait(false);

    //    return response.IsSuccessStatusCode;
    //}

    //public async Task<bool> PutAsJsonAsync<T>(string uri, T item)
    //{
    //    if (item == null)
    //    {
    //        return false;
    //    }

    //    var serializedItem = JsonConvert.SerializeObject(item);

    //    var response = await _client.PutAsync(uri, new StringContent(serializedItem, Encoding.UTF8, "application/json")).ConfigureAwait(false);

    //    return response.IsSuccessStatusCode;
    //}


    // Add this to all public methods
    private void AddAuthorizationHeader(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _client.DefaultRequestHeaders.Authorization = null;
            return;
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}

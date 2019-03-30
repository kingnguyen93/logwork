using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogWork
{
    public interface IRestClient
    {
        Task<HttpResponseMessage> GetAsync(string url, CancellationToken? token = null);

        Task<T> GetAsync<T>(string url, CancellationToken? token = null, JsonSerializerSettings deserializeSettings = null);

        Task<byte[]> GetByteArrayAsync(string url);

        Task<Stream> GetStreamAsync(string url);

        Task<string> GetStringAsync(string url);

        Task<T> GetStringAsync<T>(string url, JsonSerializerSettings deserializeSettings = null);

        Task<HttpResponseMessage> PostAsync<T>(string url, T t, CancellationToken? token = null, JsonSerializerSettings serializeSettings = null);

        Task<T> PostAsync<T, TP>(string url, TP t, CancellationToken? token = null, JsonSerializerSettings serializeSettings = null, JsonSerializerSettings deserializeSettings = null);

        Task<bool> PutAsync<T>(string url, T t, JsonSerializerSettings serializeSettings = null);

        Task<bool> DeleteAsync(string url);
    }
}
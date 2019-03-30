using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Converters;

namespace LogWork
{
    public class RestClient : IRestClient
    {
        private readonly int timeOut = 60;

        private readonly JsonSerializerSettings DefaultSerializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>() { new HandleEmtryGuidConverter(), new DateToUnixConverter() }
        };

        private readonly JsonSerializerSettings DefaultDeserializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> { new IgnoreDataTypeConverter() }
        };

        public RestClient()
        {
        }

        public RestClient(int timeOut)
        {
            this.timeOut = timeOut;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, CancellationToken? token = null)
        {
            Debug.WriteLine("GET: " + url);

            if (token != null)
                return await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.GetAsync(url, token.Value);
            else
                return await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.GetAsync(url);
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken? token = null, JsonSerializerSettings deserializeSettings = null)
        {
            Debug.WriteLine("GET: " + url);

            HttpResponseMessage responseMessage;

            if (token != null)
                responseMessage = await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.GetAsync(url, token.Value);
            else
                responseMessage = await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.GetAsync(url);

            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            Debug.WriteLine("RESPONSE: " + responseContent);

            return JsonConvert.DeserializeObject<T>(responseContent, deserializeSettings ?? DefaultDeserializeSettings);
        }

        public async Task<byte[]> GetByteArrayAsync(string url)
        {
            Debug.WriteLine("GET: " + url);

            return await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.GetByteArrayAsync(url);
        }

        public async Task<Stream> GetStreamAsync(string url)
        {
            Debug.WriteLine("GET: " + url);

            return await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.GetStreamAsync(url);
        }

        public async Task<string> GetStringAsync(string url)
        {
            Debug.WriteLine("GET: " + url);

            return await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.GetStringAsync(url);
        }

        public async Task<T> GetStringAsync<T>(string url, JsonSerializerSettings deserializeSettings = null)
        {
            Debug.WriteLine("GET: " + url);

            var responseContent = await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.GetStringAsync(url);

            Debug.WriteLine("RESPONSE: " + responseContent);

            return JsonConvert.DeserializeObject<T>(responseContent, deserializeSettings ?? DefaultDeserializeSettings);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T t, CancellationToken? token = null, JsonSerializerSettings serializerSettings = null)
        {
            Debug.WriteLine("POST: " + url);

            var content = JsonConvert.SerializeObject(t, serializerSettings ?? DefaultSerializeSettings);

            Debug.WriteLine("CONTENT: " + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (token != null)
                return await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.PostAsync(url, httpContent, token.Value);
            else
                return await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.PostAsync(url, httpContent);
        }

        public async Task<T> PostAsync<T, TP>(string url, TP t, CancellationToken? token = null, JsonSerializerSettings serializerSettings = null, JsonSerializerSettings deserializeSettings = null)
        {
            Debug.WriteLine("POST: " + url);

            var content = JsonConvert.SerializeObject(t, serializerSettings ?? DefaultSerializeSettings);

            Debug.WriteLine("CONTENT: " + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage responseMessage;

            if (token != null)
                responseMessage = await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.PostAsync(url, httpContent, token.Value);
            else
                responseMessage = await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.PostAsync(url, httpContent);

            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            Debug.WriteLine("RESPONSE: " + responseContent);

            return JsonConvert.DeserializeObject<T>(responseContent, deserializeSettings ?? DefaultDeserializeSettings);
        }

        public async Task<bool> PutAsync<T>(string url, T t, JsonSerializerSettings serializerSettings = null)
        {
            Debug.WriteLine("PUT: " + url);

            var content = JsonConvert.SerializeObject(t, serializerSettings ?? DefaultSerializeSettings);

            Debug.WriteLine("CONTENT: " + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return (await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.PutAsync(url, httpContent)).IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string url)
        {
            Debug.WriteLine("DELETE: " + url);

            return (await new HttpClient() { Timeout = TimeSpan.FromSeconds(timeOut) }.DeleteAsync(url)).IsSuccessStatusCode;
        }
    }
}
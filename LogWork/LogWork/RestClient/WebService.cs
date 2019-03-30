using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Converters;

namespace LogWork
{
    public class WebService
    {
        private static readonly Lazy<WebService> lazy = new Lazy<WebService>(() => new WebService());

        public static WebService Instance => lazy.Value;

        private readonly IRestClient restClient;

        private readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>() { new HandleEmtryGuidConverter(), new DateToUnixConverter() }
        };

        private readonly JsonSerializerSettings DefaultDeserializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> { new IgnoreDataTypeConverter() }
        };

        private WebService()
        {
            restClient = TinyIoC.TinyIoCContainer.Current.Resolve<IRestClient>();
        }

        public async Task<T> PostMediaAsync<T>(string url, string content, CancellationToken? token = null)
        {
            Debug.WriteLine("POST: " + url);

            var bytes = Convert.FromBase64String(content);
            HttpContent httpContent = new StreamContent(new MemoryStream(bytes, 0, bytes.Length));
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            Debug.WriteLine("CONTENT: " + content);

            HttpResponseMessage responseMessage;

            using (var client = new HttpClient())
            {
                if (token != null)
                    responseMessage = await client.PostAsync(url, httpContent, token.Value);
                else
                    responseMessage = await client.PostAsync(url, httpContent);
            }

            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            Debug.WriteLine("RESPONSE: " + responseContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(responseContent, new IgnoreDataTypeConverter());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: " + ex);
                    return default;
                }
            }
            else
            {
                return default;
            }
        }
    }
}
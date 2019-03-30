using Newtonsoft.Json;

namespace LogWork.Models.Response
{
    public class SyncResponse
    {
        [JsonProperty("SUCCESS")]
        public string Success { get; set; }
    }
}
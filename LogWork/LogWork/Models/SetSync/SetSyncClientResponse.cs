using Newtonsoft.Json;

namespace LogWork.Models.SetSync
{
    public class SetSyncClientResponse
    {
        [JsonProperty("appli_id")]
        public string AppId { get; set; }

        [JsonProperty("server_id")]
        public int ServerId { get; set; }

        [JsonProperty("code_id")]
        public int CodeId { get; set; }
    }
}
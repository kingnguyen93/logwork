using Newtonsoft.Json;

namespace LogWork.Models.SetSync
{
    // [0:] RESPONSE: {"SUCCESS":"OK","media_id":"196220","media_code_id":"1562","media_link_id":"726450"}
    public class SetMediaResponse
    {
        [JsonProperty("SUCCESS")]
        public string Success { get; set; }

        [JsonProperty("media_id")]
        public int MediaId { get; set; }

        [JsonProperty("media_code_id")]
        public int MediaCodeId { get; set; }

        [JsonProperty("media_file_name")]
        public string MediaFileName { get; set; }

        [JsonProperty("media_link_id")]
        public int MediaLinkId { get; set; }
    }
}
using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class MediaLinkResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int FkMediaServerId { get; set; }

        [JsonProperty("3")]
        public string LinkTable { get; set; }

        [JsonProperty("4")]
        public int FkColumnServerId { get; set; }

        [JsonProperty("5")]
        public int IsActif { get; set; }

        [JsonProperty("6")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
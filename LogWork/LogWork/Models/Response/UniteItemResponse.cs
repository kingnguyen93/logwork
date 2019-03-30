using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class UniteItemResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int UniteServerId { get; set; }

        [JsonProperty("3")]
        public string Value { get; set; }

        [JsonProperty("4")]
        public int IsActif { get; set; }

        [JsonProperty("5")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
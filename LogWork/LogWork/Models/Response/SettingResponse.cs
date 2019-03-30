using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class SettingResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public string Name { get; set; }

        [JsonProperty("2")]
        public string Value { get; set; }

        [JsonProperty("3")]
        public int? IsActif { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
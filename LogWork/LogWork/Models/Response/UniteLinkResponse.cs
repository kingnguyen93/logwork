using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class UniteLinkResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int FkUniteServerId { get; set; }

        [JsonProperty("3")]
        public string LinkTableName { get; set; }

        [JsonProperty("4")]
        public int FkColumnServerId { get; set; }

        [JsonProperty("5")]
        public string UniteValue { get; set; }

        [JsonProperty("6")]
        public int IsActif { get; set; }

        [JsonProperty("7")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
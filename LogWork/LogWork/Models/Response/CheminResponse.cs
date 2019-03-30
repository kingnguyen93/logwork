using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class CheminResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int Code { get; set; }

        [JsonProperty("3")]
        public string Nom { get; set; }

        [JsonProperty("4")]
        public string Comment { get; set; }

        [JsonProperty("5")]
        public int IsActif { get; set; }

        [JsonProperty("6")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
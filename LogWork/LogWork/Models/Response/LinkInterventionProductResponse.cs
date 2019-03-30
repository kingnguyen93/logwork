using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class LinkInterventionProductResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int FkProductServerId { get; set; }

        [JsonProperty("3")]
        public int FkInterventionServerId { get; set; }

        [JsonProperty("4")]
        public decimal Quantity { get; set; }

        [JsonProperty("5")]
        public int Position { get; set; }

        [JsonProperty("6")]
        public int IsActif { get; set; }

        [JsonProperty("7")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("10")]
        public string ProductName { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class UniteResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public string TypeStr { get; set; }

        [JsonProperty("3")]
        public string Nom { get; set; }

        [JsonProperty("4")]
        public int IsAlwaysVisible { get; set; }

        [JsonProperty("5")]
        public int IsActif { get; set; }

        [JsonProperty("6")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("7")]
        public int FilialeServerKey { get; set; }

        [JsonProperty("8")]
        public int FieldType { get; set; }

        [JsonProperty("9")]
        public int Position { get; set; }

        [JsonProperty("10")]
        public int ParentId { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
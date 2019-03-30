using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class FilialeResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int Code { get; set; }

        [JsonProperty("3")]
        public string Adresse { get; set; }

        [JsonProperty("4")]
        public string CodePostal { get; set; }

        [JsonProperty("5")]
        public string Ville { get; set; }

        [JsonProperty("6")]
        public string Phone { get; set; }

        [JsonProperty("7")]
        public string Fax { get; set; }

        [JsonProperty("8")]
        public string Nom { get; set; }

        [JsonProperty("9")]
        public double Longitude { get; set; }

        [JsonProperty("10")]
        public double Latitude { get; set; }

        [JsonProperty("11")]
        public string Comment { get; set; }

        [JsonProperty("12")]
        public int IsFavorite { get; set; }

        [JsonProperty("13")]
        public int IsActif { get; set; }

        [JsonProperty("14")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
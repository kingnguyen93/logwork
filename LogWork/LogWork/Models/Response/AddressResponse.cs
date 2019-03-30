using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class AddressResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int Code { get; set; }

        [JsonProperty("3")]
        public int FkClientServerId { get; set; }

        [JsonProperty("4")]
        public int FkCheminServerId { get; set; }

        [JsonProperty("6")]
        public string Prenom { get; set; }

        [JsonProperty("7")]
        public string Nom { get; set; }

        [JsonProperty("8")]
        public string Societe { get; set; }

        [JsonProperty("9")]
        public string Adresse { get; set; }

        [JsonProperty("10")]
        public string CodePostal { get; set; }

        [JsonProperty("11")]
        public string Ville { get; set; }

        [JsonProperty("12")]
        public string DepartmentFr { get; set; }

        [JsonProperty("13")]
        public string Region { get; set; }

        [JsonProperty("14")]
        public double Longitude { get; set; }

        [JsonProperty("15")]
        public double Latitude { get; set; }

        [JsonProperty("16")]
        public double Altitude { get; set; }

        [JsonProperty("17")]
        public string HeureLundi { get; set; }

        [JsonProperty("18")]
        public string HeureMardi { get; set; }

        [JsonProperty("19")]
        public string HeureMercredi { get; set; }

        [JsonProperty("20")]
        public string HeureJeudi { get; set; }

        [JsonProperty("21")]
        public string HeureVendredi { get; set; }

        [JsonProperty("22")]
        public string HeureSamedi { get; set; }

        [JsonProperty("23")]
        public string HeureDimanche { get; set; }

        [JsonProperty("24")]
        public int IsBonPassage { get; set; }

        [JsonProperty("25")]
        public int IsCle { get; set; }

        [JsonProperty("26")]
        public string Comment { get; set; }

        [JsonProperty("27")]
        public int IsActif { get; set; }

        [JsonProperty("28")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
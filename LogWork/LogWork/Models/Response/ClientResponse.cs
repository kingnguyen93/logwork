using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class ClientResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int Code { get; set; }

        [JsonProperty("3")]
        public int FkMainAdressesServerId { get; set; }

        [JsonProperty("4")]
        public string Title { get; set; }

        [JsonProperty("5")]
        public int Civilite { get; set; }

        [JsonProperty("6")]
        public string Prenom { get; set; }

        [JsonProperty("7")]
        public string Nom { get; set; }

        [JsonProperty("8")]
        public string Societe { get; set; }

        [JsonProperty("9")]
        public string Email { get; set; }

        [JsonProperty("10")]
        public string PhoneFixe { get; set; }

        [JsonProperty("11")]
        public string PhoneMobile { get; set; }

        [JsonProperty("12")]
        public string PhonePro { get; set; }

        [JsonProperty("13")]
        public string Fax { get; set; }

        [JsonProperty("14")]
        public string Lang { get; set; }

        [JsonProperty("15")]
        public string Comment { get; set; }

        [JsonProperty("16")]
        public int IsActif { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
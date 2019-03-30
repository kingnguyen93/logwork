using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class UserResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int Code { get; set; }

        [JsonProperty("3")]
        public int FkTeamServerId { get; set; }

        [JsonProperty("4")]
        public string Login { get; set; }

        [JsonProperty("5")]
        public string Rang { get; set; }

        [JsonProperty("6")]
        public string Civilite { get; set; }

        [JsonProperty("7")]
        public string Prenom { get; set; }

        [JsonProperty("8")]
        public string Nom { get; set; }

        [JsonProperty("9")]
        public string Email { get; set; }

        [JsonProperty("10")]
        public string Lang { get; set; }

        [JsonProperty("11")]
        public string Phone { get; set; }

        [JsonProperty("12")]
        public int IsActif { get; set; }

        [JsonProperty("13")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
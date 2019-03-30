using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class MessageResponse
    {
        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("3")]
        public int FkUserServerIdFrom { get; set; }

        [JsonProperty("4")]
        public int FkUserServerIdTo { get; set; }

        [JsonProperty("5")]
        public int FkMessagerieServerId { get; set; }

        [JsonProperty("6")]
        public string Title { get; set; }

        [JsonProperty("7")]
        public string Content { get; set; }

        [JsonProperty("8")]
        public int IsFavorite { get; set; }

        [JsonProperty("9")]
        public int IsDraft { get; set; }

        [JsonProperty("10")]
        public int IsRead { get; set; }

        [JsonProperty("11")]
        public int IsDelete { get; set; }

        [JsonProperty("12")]
        public int IsActif { get; set; }

        [JsonProperty("13")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
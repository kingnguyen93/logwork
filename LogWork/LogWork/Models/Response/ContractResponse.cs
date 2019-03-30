using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class ContractResponse
    {
        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int CodeId { get; set; }

        [JsonProperty("3")]
        public int FkClientServerId { get; set; }

        [JsonProperty("4")]
        public int FkAddressServerId { get; set; }

        [JsonProperty("5")]
        public int FkFilialeId { get; set; }

        [JsonProperty("6")]
        public string Title { get; set; }

        [JsonProperty("7")]
        public string ConCode { get; set; }

        [JsonProperty("8")]
        public DateTime? ConDateStart { get; set; } //contract date start

        [JsonProperty("9")]
        public DateTime? ConDateEnd { get; set; } // contract date end

        [JsonProperty("10")]
        public int ConMinute { get; set; } //Amount of minute in contract

        [JsonProperty("11")]
        public Decimal ConBudget { get; set; }

        [JsonProperty("14")]
        public int IsClosed { get; set; } // 0 : open , 1: closed

        [JsonProperty("12")]
        public int IsActif { get; set; }

        [JsonProperty("13")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class MediaResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int Code { get; set; }

        [JsonProperty("3")]
        public int AccountId { get; set; }

        [JsonProperty("4")]
        public string Year { get; set; }

        [JsonProperty("5")]
        public string Month { get; set; }

        [JsonProperty("6")]
        public string FileName { get; set; }

        [JsonProperty("7")]
        public int FileSize { get; set; }

        [JsonProperty("8")]
        public string FileMime { get; set; }

        [JsonProperty("9")]
        public int ImageWidth { get; set; }

        [JsonProperty("10")]
        public int ImageHeight { get; set; }

        [JsonProperty("11")]
        public string Legend { get; set; }

        [JsonProperty("12")]
        public int IsFav { get; set; }

        [JsonProperty("13")]
        public int IsActif { get; set; }

        [JsonProperty("14")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
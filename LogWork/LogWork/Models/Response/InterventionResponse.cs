using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class InterventionResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int Code { get; set; }

        [JsonProperty("3")]
        public int FkUserServerlId { get; set; }

        [JsonProperty("4")]
        public int FkClientServerId { get; set; }

        [JsonProperty("5")]
        public int FkAdresseServerId { get; set; }

        [JsonProperty("6")]
        public int FkCheminServerId { get; set; }

        [JsonProperty("7")]
        public string Nom { get; set; }

        [JsonProperty("8")]
        public int Priority { get; set; }

        [JsonProperty("9")]
        public int IsDone { get; set; }

        [JsonProperty("10")]
        public int IsActif { get; set; }

        [JsonProperty("11")]
        public int FkParentServerlId { get; set; }

        [JsonProperty("12")]
        public DateTime? PlanningDateStart { get; set; }

        [JsonProperty("13")]
        public DateTime? PlanningDateEnd { get; set; }

        [JsonProperty("14")]
        public string PlanningHourStart { get; set; }

        [JsonProperty("15")]
        public string PlanningHourEnd { get; set; }

        [JsonProperty("16")]
        public string PlanningHour { get; set; }

        [JsonProperty("17")]
        public string PlanningComment { get; set; }

        [JsonProperty("18")]
        public DateTime? DoneDateStart { get; set; }

        [JsonProperty("19")]
        public DateTime? DoneDateEnd { get; set; }

        [JsonProperty("20")]
        public string DoneHourStart { get; set; }

        [JsonProperty("21")]
        public string DoneHourEnd { get; set; }

        [JsonProperty("22")]
        public string DoneHour { get; set; }

        [JsonProperty("23")]
        public string DoneComment { get; set; }

        [JsonProperty("24")]
        public double DoneLongitude { get; set; }

        [JsonProperty("25")]
        public double DoneLatitude { get; set; }

        [JsonProperty("26")]
        public double DoneAltitude { get; set; }

        [JsonProperty("27")]
        public string Undefined { get; set; }

        [JsonProperty("28")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("29")]
        public int FkFilialeServerId { get; set; }

        [JsonProperty("30")]
        public string Nonce { get; set; }

        [JsonProperty("31")]
        public int FkContratServerId { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
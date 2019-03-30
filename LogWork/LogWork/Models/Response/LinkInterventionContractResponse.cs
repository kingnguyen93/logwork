using System;
using Newtonsoft.Json;

namespace Organilog.Models.Response
{
    public class LinkInterventionContractResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int FkTaskServerId { get; set; }

        [JsonProperty("3")]
        public int FkInterventionServerId { get; set; }

        [JsonProperty("4")]
        public int IsPlanningToDo { get; set; }

        [JsonProperty("5")]
        public int IsDone { get; set; }

        [JsonProperty("6")]
        public int IsActif { get; set; }

        [JsonProperty("7")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("8")]
        public int PlanningMinute { get; set; }

        [JsonProperty("9")]
        public int DoneMinute { get; set; }

        [JsonProperty("10")]
        public string Nonce { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace LogWork.Models.Response
{
    public class InterventionHistoryRespone
    {
        [JsonProperty("interventions")]
        public List<InterventionResponse> Interventions { get; set; }
    }
}
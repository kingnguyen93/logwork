using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Forms.Converters;

namespace LogWork.Models.SetSync
{
    public class SetSyncResponse
    {
        [JsonProperty("SUCCESS")]
        public string Success { get; set; }

        [JsonProperty("clients")]
        public List<SetSyncClientResponse> Clients { get; set; } = new List<SetSyncClientResponse>();

        [JsonProperty("adresses")]
        public List<SetSyncAdresseResponse> Adresses { get; set; } = new List<SetSyncAdresseResponse>();

        [JsonProperty("interventions")]
        public List<SetSyncInterventionResponse> Interventions { get; set; } = new List<SetSyncInterventionResponse>();

        [JsonProperty("messageries")]
        public List<SetSyncMessageResponse> Messageries { get; set; } = new List<SetSyncMessageResponse>();

        [JsonProperty("lits")]
        public List<SetSyncLitResponse> Lits { get; set; } = new List<SetSyncLitResponse>();

        [JsonProperty("lips")]
        public List<SetSyncLipResponse> Lips { get; set; } = new List<SetSyncLipResponse>();

        [JsonProperty("unite_links")]
        public List<SetSyncUniteLinkResponse> UniteLinks { get; set; } = new List<SetSyncUniteLinkResponse>();

        [JsonProperty("users")]
        [JsonConverter(typeof(HandleDataTypeConverter))]
        public List<SetSyncUserResponse> Users { get; set; } = new List<SetSyncUserResponse>();
    }
}
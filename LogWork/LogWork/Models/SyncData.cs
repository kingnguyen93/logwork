using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;

namespace LogWork.Models
{
    public partial class SyncData
    {
        [JsonProperty("i")]
        public string I { get; set; }

        [JsonProperty("v")]
        public Dictionary<string, string> V { get; set; }
    }

    public partial class SyncData
    {
        public static Dictionary<string, List<SyncData>> FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, List<SyncData>>>(json, Converter.SyncDataSettings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this Dictionary<string, List<SyncData>> self) => JsonConvert.SerializeObject(self, Converter.SyncDataSettings);
    }

    internal static partial class Converter
    {
        public static readonly JsonSerializerSettings SyncDataSettings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeLocal }
            },
        };
    }
}
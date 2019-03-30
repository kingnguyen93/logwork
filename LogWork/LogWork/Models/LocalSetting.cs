using Newtonsoft.Json;
using System.Collections.Generic;

namespace LogWork.Models
{
    public class DefaultSetting
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("items")]
        public Dictionary<string, LocalSetting> Items { get; set; }
    }

    public class LocalSetting
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("arrange")]
        public List<string> Arrange { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("order")]
        public string Order { get; set; }
    }
}
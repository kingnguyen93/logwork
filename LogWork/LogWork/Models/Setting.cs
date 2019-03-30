using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using System.Collections.Generic;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Setting")]
    public class Setting : TinyModel
    {
        [JsonProperty("seId")]
        public Guid Id { get; set; }

        [JsonProperty("seIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("seUserId")]
        public int UserId { get; set; }

        [JsonProperty("seCategory")]
        public string Category { get; set; } = "";

        [JsonProperty("seName")]
        [PrimaryKey]
        public string Name { get; set; }

        private string _value;
        [JsonProperty("seValue")]
        public string Value { get => _value; set =>SetProperty(ref _value, value); }

        [JsonProperty("seDefaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("seType")]
        public string Type { get; set; } = "";

        [JsonProperty("seArrange")]
        [Ignore]
        public List<string> Arrange { get; set; }

        [JsonProperty("seMessage")]
        public string Message { get; set; }

        [JsonProperty("seDescription")]
        public string Description { get; set; }

        [JsonProperty("seOrder")]
        public string Order { get; set; }

        [JsonProperty("seIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("seSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("seAddDated")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("seEditDate")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("seLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        [Ignore]
        public List<SettingItem> SettingItems { get; set; }

        public bool IsToSync { get; set; }

        public Setting()
        {
        }

        public Setting(SettingResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            Name = response.Name;
            Value = response.Value;
            IsActif = response?.IsActif ?? 1;
        }

        public void UpdateFromResponse(SettingResponse response)
        {
            Name = response.Name;
            Value = response.Value;
            IsActif = response?.IsActif ?? 1;
        }
    }

    [Table("SettingItem")]
    public class SettingItem
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
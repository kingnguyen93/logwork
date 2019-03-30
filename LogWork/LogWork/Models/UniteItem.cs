using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("UniteItem")]
    public class UniteItem : TinyModel
    {
        [JsonProperty("uiId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("uiIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("uiUserId")]
        public int UserId { get; set; }

        [JsonProperty("uiUniteUUID")]
        public Guid UniteAppId { get; set; }

        [JsonProperty("uiUniteServerId")]
        public int UniteServerId { get; set; }

        [JsonProperty("uiValue")]
        public string Value { get; set; }

        [JsonProperty("uiIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("uiOn")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("uiModifOn")]
        public DateTime? EditDate { get; set; }

        [Ignore]
        public bool Selected { get; set; }

        public bool IsToSync { get; set; }

        public UniteItem()
        {
        }

        public UniteItem(UniteItemResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            UniteServerId = response.UniteServerId;
            Value = response.Value;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }

        public void UpdateFromResponse(UniteItemResponse response)
        {
            UniteServerId = response.UniteServerId;
            Value = response.Value;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }
    }
}
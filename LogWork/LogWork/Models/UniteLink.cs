using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("UniteLink")]
    public class UniteLink : TinyModel
    {
        [JsonProperty("ulId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("ulIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("ulUserId")]
        public int UserId { get; set; }

        [JsonProperty("ulFkUnUUID")]
        public Guid FkUniteAppliId { get; set; }

        [JsonProperty("ulFkUnIdServer")]
        public int FkUniteServerId { get; set; }

        [JsonProperty("ulLinkTable")]
        public string LinkTableName { get; set; }

        [JsonProperty("ulFkColUUID")]
        public Guid FkColumnAppliId { get; set; }

        [JsonProperty("ulFkColIdServer")]
        public int FkColumnServerId { get; set; }

        [JsonProperty("ulValue")]
        public string UniteValue { get; set; }

        [JsonProperty("ulOn")]
        public int IsActif { get; set; }

        [JsonProperty("ulSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("ulAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("ulModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("ulLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        [Ignore]
        public Unite Unite { get; set; }

        [Ignore]
        public string UniteTitle { get; set; }

        [Ignore]
        public string UniteDisplay { get; set; }

        [Ignore]
        public int UniteType { get; set; }

        public bool IsToSync { get; set; }

        public UniteLink()
        {
        }

        public UniteLink(UniteLinkResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            FkUniteServerId = response.FkUniteServerId;
            LinkTableName = response.LinkTableName;
            FkColumnServerId = response.FkColumnServerId;
            UniteValue = response.UniteValue;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }

        public void UpdateFromResponse(UniteLinkResponse response)
        {
            FkUniteServerId = response.FkUniteServerId;
            LinkTableName = response.LinkTableName;
            FkColumnServerId = response.FkColumnServerId;
            UniteValue = response.UniteValue;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }
    }
}
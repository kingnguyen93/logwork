using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("MediaLink")]
    public class MediaLink : TinyModel
    {
        [JsonProperty("ulAppId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("ulId")]
        public int ServerId { get; set; }

        [JsonProperty("ulUserId")]
        public int UserId { get; set; }

        [JsonProperty("ulCode")]
        public int Code { get; set; }

        [JsonProperty("ulMediaUUID")]
        public Guid FkMediaAppliId { get; set; }

        [JsonProperty("ulMediaServerId")]
        public int FkMediaServerId { get; set; }

        [JsonProperty("ulLinkTable")]
        public string LinkTable { get; set; }

        [JsonProperty("ulColumnUUID")]
        public Guid FkColumnAppliId { get; set; }

        [JsonProperty("ulColumnServerId")]
        public long FkColumnServerId { get; set; }

        private int isActif;
        [JsonProperty("ulActif")]
        public int IsActif { get => isActif; set => SetProperty(ref isActif, value); }

        [JsonProperty("ulSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("ulCreatedOn")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("ulModifDate")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("ulLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        [Ignore]
        public Media Media { get; set; }

        [JsonIgnore]
        public bool IsDelete { get; set; }

        public bool IsToSync { get; set; }

        public MediaLink()
        {
        }

        public MediaLink(MediaLinkResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            FkMediaServerId = response.FkMediaServerId;
            LinkTable = response.LinkTable;
            FkColumnServerId = response.FkColumnServerId;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }

        public void UpdateFromResponse(MediaLinkResponse response)
        {
            FkMediaServerId = response.FkMediaServerId;
            LinkTable = response.LinkTable;
            FkColumnServerId = response.FkColumnServerId;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }
    }
}
using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("CategoryTracking")]
    public class CategoryTracking : TinyModel
    {
        [JsonProperty("ctId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("ctIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("ctUserId")]
        public int UserId { get; set; }

        [JsonProperty("ctTypeStr")]
        public int Code { get; set; }

        [JsonProperty("ctNom")]
        public string Title { get; set; }

        [JsonProperty("ctIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("ctSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("ctAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("ctModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("ctLastViewDate")]
        public DateTime? LastViewDate { get; set; }
        
        public bool IsToSync { get; set; }

        public CategoryTracking()
        {
        }

        public CategoryTracking(CategoryTrackingResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            Code = response.Code;
            Title = response.Title;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }

        public void UpdateFromResponse(CategoryTrackingResponse response)
        {
            Code = response.Code;
            Title = response.Title;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }
    }
}
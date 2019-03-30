using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Task")]
    public class Tasks : TinyModel
    {
        [JsonProperty("taId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("taServerId")]
        public int ServerId { get; set; }

        [JsonProperty("taUserId")]
        public int UserId { get; set; }

        [JsonProperty("taCode")]
        public int Code { get; set; }

        [JsonProperty("taFilialeServerKey")]
        public int FilialeServerKey { get; set; }

        [JsonProperty("taNom")]
        public string Nom { get; set; }

        [JsonProperty("taOrder")]
        public string Order { get; set; }

        [JsonProperty("taComment")]
        public string Comment { get; set; }

        [JsonProperty("taSortIndex")]
        public string SortIndex { get; set; }

        [JsonProperty("taIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("taSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("taAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("taEditDate")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("taLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        public bool Selected { get; set; }

        public TimeSpan Time { get; set; }

        public int Minute { get; set; }

        public bool IsToSync { get; set; }

        public Tasks()
        {
        }

        public Tasks(TaskResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            Code = response.Code;
            Nom = response.Nom;
            Comment = response.Comment;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
            SortIndex = response.SortIndex;
            FilialeServerKey = response.FilialeServerKey;
        }

        public void UpdateFromResponse(TaskResponse response)
        {
            Code = response.Code;
            Nom = response.Nom;
            Comment = response.Comment;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
            SortIndex = response.SortIndex;
            FilialeServerKey = response.FilialeServerKey;
        }
    }
}
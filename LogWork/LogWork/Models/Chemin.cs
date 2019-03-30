using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Chemin")]
    public class Chemin : TinyModel
    {
        [JsonProperty("cheId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("cheIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("cheUserId")]
        public int UserId { get; set; }

        [JsonProperty("cheCode")]
        public int Code { get; set; }

        [JsonProperty("cheNom")]
        public string Nom { get; set; }

        [JsonProperty("cheComment")]
        public string Comment { get; set; }

        [JsonProperty("cheIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("cheSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("cheAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("cheModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("cheLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        public bool IsToSync { get; set; }

        public Chemin()
        {
        }

        public Chemin(CheminResponse response)
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
        }

        public void UpdateFromResponse(CheminResponse response)
        {
            Code = response.Code;
            Nom = response.Nom;
            Comment = response.Comment;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }
    }
}
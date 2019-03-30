using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Filiale")]
    public class Filiale : TinyModel
    {
        [JsonProperty("fiId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("fiIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("fiUserId")]
        public int UserId { get; set; }

        [JsonProperty("fiCode")]
        public int Code { get; set; }

        [JsonProperty("fiAdresse")]
        public string Adresse { get; set; }

        [JsonProperty("fiCodePostal")]
        public string CodePostal { get; set; }

        [JsonProperty("fiVille")]
        public string Ville { get; set; }

        [JsonProperty("fiPhone")]
        public string Phone { get; set; }

        [JsonProperty("fiFax")]
        public string Fax { get; set; }

        [JsonProperty("fiNom")]
        public string Nom { get; set; }

        [JsonProperty("fiLongitude")]
        public double Longitude { get; set; }

        [JsonProperty("fiLatitude")]
        public double Latitude { get; set; }

        [JsonProperty("fiComment")]
        public string Comment { get; set; }

        [JsonProperty("fiIsFavorite")]
        public int IsFavorite { get; set; }

        [JsonProperty("fiIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("fiSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("fiAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("fiModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("fiLastViewDate")]
        public DateTime? LastViewDate { get; set; }
        
        public bool IsToSync { get; set; }

        public Filiale()
        {
        }

        public Filiale(FilialeResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            Code = response.Code;
            Adresse = response.Adresse;
            CodePostal = response.CodePostal;
            Ville = response.Ville;
            Phone = response.Phone;
            Fax = response.Fax;
            Nom = response.Nom;
            Longitude = response.Longitude;
            Latitude = response.Latitude;
            Comment = response.Comment;
            IsFavorite = response.IsFavorite;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }

        public void UpdateFromResponse(FilialeResponse response)
        {
            Code = response.Code;
            Adresse = response.Adresse;
            CodePostal = response.CodePostal;
            Ville = response.Ville;
            Phone = response.Phone;
            Fax = response.Fax;
            Nom = response.Nom;
            Longitude = response.Longitude;
            Latitude = response.Latitude;
            Comment = response.Comment;
            IsFavorite = response.IsFavorite;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }
    }
}
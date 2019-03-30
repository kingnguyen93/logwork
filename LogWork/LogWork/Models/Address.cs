using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Address")]
    public class Address : TinyModel
    {
        [JsonProperty("aId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("aIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("aUserId")]
        public int UserId { get; set; }

        [JsonProperty("aCode")]
        public int Code { get; set; }

        [JsonProperty("aFkClientAppliId")]
        public Guid FkClientAppliId { get; set; }

        [JsonProperty("aFkClientId")]
        public int FkClientServerId { get; set; }

        [JsonProperty("aFkCheminAppliId")]
        public Guid FkCheminAppliId { get; set; }

        [JsonProperty("aFkCheminId")]
        public int FkCheminServerId { get; set; }

        [JsonProperty("aPays")]
        public string Pays { get; set; }

        [JsonProperty("aPrenom")]
        public string Prenom { get; set; }

        [JsonProperty("aNom")]
        public string Nom { get; set; }

        [JsonProperty("aSociete")]
        public string Societe { get; set; }

        [JsonProperty("aAdresse")]
        public string Adresse { get; set; }

        [JsonProperty("aCodePostal")]
        public string CodePostal { get; set; }

        [JsonProperty("aVille")]
        public string Ville { get; set; }

        [JsonProperty("aFrDept")]
        public string DepartmentFr { get; set; }

        [JsonProperty("aRegion")]
        public string Region { get; set; }

        [JsonProperty("aLong")]
        public double Longitude { get; set; }

        [JsonProperty("aLat")]
        public double Latitude { get; set; }

        [JsonProperty("aAlt")]
        public double Altitude { get; set; }

        [JsonProperty("aLocM")]
        public string LocalisationMethod { get; set; }

        [JsonProperty("aHeure1")]
        public string HeureLundi { get; set; }

        [JsonProperty("aHeure2")]
        public string HeureMardi { get; set; }

        [JsonProperty("aHeure3")]
        public string HeureMercredi { get; set; }

        [JsonProperty("aHeure4")]
        public string HeureJeudi { get; set; }

        [JsonProperty("aHeure5")]
        public string HeureVendredi { get; set; }

        [JsonProperty("aHeure6")]
        public string HeureSamedi { get; set; }

        [JsonProperty("aHeure7")]
        public string HeureDimanche { get; set; }

        [JsonProperty("aIsCle")]
        public int IsCle { get; set; }

        [JsonProperty("aIsBonPassage")]
        public int IsBonPassage { get; set; }

        [JsonProperty("aComment")]
        public string Comment { get; set; }

        [JsonProperty("aOn")]
        public int IsActif { get; set; }

        [JsonProperty("aSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("aAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("aModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("aLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        [Ignore]
        [JsonIgnore]
        public string FullAddress => (Adresse + " " + CodePostal + " " + Ville)?.Trim();

        [Ignore]
        [JsonIgnore]
        public string FullName => (Prenom + " " + Nom)?.Trim();

        [Ignore]
        [JsonIgnore]
        public bool IsLocationSet => Latitude > 0 || Longitude > 0;

        public bool IsToSync { get; set; }

        public Address()
        {
        }

        public Address(AddressResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            Code = response.Code;
            FkClientServerId = response.FkClientServerId;
            FkCheminServerId = response.FkCheminServerId;
            Prenom = response.Prenom;
            Nom = response.Nom;
            Societe = response.Societe;
            Adresse = response.Adresse;
            CodePostal = response.CodePostal;
            Ville = response.Ville;
            DepartmentFr = response.DepartmentFr;
            Region = response.Region;
            Longitude = response.Longitude;
            Latitude = response.Latitude;
            Altitude = response.Altitude;
            HeureLundi = response.HeureLundi;
            HeureMardi = response.HeureMardi;
            HeureMercredi = response.HeureMercredi;
            HeureJeudi = response.HeureJeudi;
            HeureVendredi = response.HeureVendredi;
            HeureSamedi = response.HeureSamedi;
            HeureDimanche = response.HeureDimanche;
            IsBonPassage = response.IsBonPassage;
            IsCle = response.IsCle;
            Comment = response.Comment;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }

        public void UpdateFromResponse(AddressResponse response)
        {
            Code = response.Code;
            FkClientServerId = response.FkClientServerId;
            FkCheminServerId = response.FkCheminServerId;
            Prenom = response.Prenom;
            Nom = response.Nom;
            Societe = response.Societe;
            Adresse = response.Adresse;
            CodePostal = response.CodePostal;
            Ville = response.Ville;
            DepartmentFr = response.DepartmentFr;
            Region = response.Region;
            Longitude = response.Longitude;
            Latitude = response.Latitude;
            Altitude = response.Altitude;
            HeureLundi = response.HeureLundi;
            HeureMardi = response.HeureMardi;
            HeureMercredi = response.HeureMercredi;
            HeureJeudi = response.HeureJeudi;
            HeureVendredi = response.HeureVendredi;
            HeureSamedi = response.HeureSamedi;
            HeureDimanche = response.HeureDimanche;
            IsBonPassage = response.IsBonPassage;
            IsCle = response.IsCle;
            Comment = response.Comment;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }
    }
}
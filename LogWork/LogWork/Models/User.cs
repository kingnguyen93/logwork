using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("User")]
    public class User : TinyModel
    {
        [JsonProperty("uId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("uIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("uUserId")]
        public int UserId { get; set; }

        [JsonProperty("uCode")]
        public int Code { get; set; }

        [JsonProperty("uFkTeamAppliId")]
        public Guid FkTeamAppliId { get; set; }

        [JsonProperty("uFkTeamServerId")]
        public int FkTeamServerId { get; set; }

        [JsonProperty("uFkFilialeAppliId")]
        public Guid FkFilialeAppliId { get; set; }

        [JsonProperty("uFkFilialeServerId")]
        public string FkFilialeServerId { get; set; }

        [JsonProperty("uLogin")]
        public string Login { get; set; }

        [JsonProperty("uRang")]
        public string Rang { get; set; }

        [JsonProperty("uCivilite")]
        public string Civilite { get; set; }

        [JsonProperty("uPrenom")]
        public string Prenom { get; set; }

        [JsonProperty("uNom")]
        public string Nom { get; set; }

        [JsonProperty("uEmail")]
        public string Email { get; set; }

        [JsonProperty("uPhone")]
        public string Phone { get; set; }

        [JsonProperty("uLang")]
        public string Lang { get; set; }

        [JsonProperty("uComment")]
        public string Comment { get; set; }

        [JsonProperty("uOn")]
        public int IsActif { get; set; }

        [JsonProperty("uSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("uAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("uModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("uLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        public string FullName => Prenom + " " + Nom;

        public bool IsToSync { get; set; }

        public User()
        {
        }

        public User(UserResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            Code = response.Code;
            FkTeamServerId = response.FkTeamServerId;
            Login = response.Login;
            Rang = response.Rang;
            Civilite = response.Civilite;
            Prenom = response.Prenom;
            Nom = response.Nom;
            Email = response.Email;
            Lang = response.Lang;
            Phone = response.Phone;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }

        public void UpdateFromResponse(UserResponse response)
        {
            FkTeamServerId = response.FkTeamServerId;
            Login = response.Login;
            Rang = response.Rang;
            Civilite = response.Civilite;
            Prenom = response.Prenom;
            Nom = response.Nom;
            Email = response.Email;
            Lang = response.Lang;
            Phone = response.Phone;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }
    }
}
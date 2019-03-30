using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using System.Collections.Generic;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Client")]
    public class Client : TinyModel
    {
        [JsonProperty("cId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("cIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("cUserId")]
        public int UserId { get; set; }

        [JsonProperty("cCode")]
        public int Code { get; set; }

        [JsonProperty("cFkAdresseMainAppliId")]
        public Guid FkMainAdressesAppliId { get; set; }

        [JsonProperty("cFkAdresseMainId")]
        public int FkMainAdressesServerId { get; set; }

        [JsonProperty("cTitle")]
        public string Title { get; set; }

        [JsonProperty("cCivilite")]
        public int Civilite { get; set; }

        [JsonProperty("cPrenom")]
        public string Prenom { get; set; }

        [JsonProperty("cNom")]
        public string Nom { get; set; }

        [JsonProperty("cSociete")]
        public string Societe { get; set; }

        [JsonProperty("cEmail")]
        public string Email { get; set; }

        [JsonProperty("cPhoneFixe")]
        public string PhoneFixe { get; set; }

        [JsonProperty("cPhoneMobile")]
        public string PhoneMobile { get; set; }

        [JsonProperty("cPhonePro")]
        public string PhonePro { get; set; }

        [JsonProperty("cFax")]
        public string Fax { get; set; }

        [JsonProperty("cLang")]
        public string Lang { get; set; }

        [JsonProperty("cComment")]
        public string Comment { get; set; }

        [JsonProperty("cOn")]
        public int IsActif { get; set; }

        [JsonProperty("cSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("cAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("cModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("cLastViewDate")]
        public DateTime? LastViewDate { get; set; }
        
        [Ignore]
        public string FullName => (Prenom + " " + Nom)?.Trim();

        private List<Address> addresses;
        [Ignore]
        public List<Address> Addresses { get => addresses; set => SetProperty(ref addresses, value, onChanged: () => { AddressesCount = addresses.Count; OnPropertyChanged(nameof(AddressesCount)); }); }
        
        [Ignore]
        public int AddressesCount { get; set; }

        public bool IsToSync { get; set; }

        public Client()
        {
        }

        public Client(ClientResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            Code = response.Code;
            FkMainAdressesServerId = response.FkMainAdressesServerId;
            Title = response.Title;
            Civilite = response.Civilite;
            Prenom = response.Prenom;
            Nom = response.Nom;
            Societe = response.Societe;
            Email = response.Email;
            PhoneFixe = response.PhoneFixe;
            PhoneMobile = response.PhoneMobile;
            PhonePro = response.PhonePro;
            Fax = response.Fax;
            Lang = response.Lang;
            Comment = response.Comment;
            IsActif = response.IsActif;
        }

        public void UpdateFromResponse(ClientResponse response)
        {
            Code = response.Code;
            FkMainAdressesServerId = response.FkMainAdressesServerId;
            Title = response.Title;
            Civilite = response.Civilite;
            Prenom = response.Prenom;
            Nom = response.Nom;
            Societe = response.Societe;
            Email = response.Email;
            PhoneFixe = response.PhoneFixe;
            PhoneMobile = response.PhoneMobile;
            PhonePro = response.PhonePro;
            Fax = response.Fax;
            Lang = response.Lang;
            Comment = response.Comment;
            IsActif = response.IsActif;
        }
    }
}
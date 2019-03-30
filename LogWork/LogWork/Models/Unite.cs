using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using System.Collections.Generic;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Unite")]
    public class Unite : TinyModel
    {
        [JsonProperty("taId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("taIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("taUserId")]
        public int UserId { get; set; }

        [JsonProperty("taFilialeServerKey")]
        public int FilialeServerKey { get; set; }

        [JsonProperty("taFieldType")]
        public int FieldType { get; set; }

        [JsonProperty("taTypeStr")]
        public string TypeStr { get; set; }

        [JsonProperty("taNom")]
        public string Nom { get; set; }

        [JsonProperty("taIsAlwaysVisible")]
        public int IsAlwaysVisible { get; set; }

        [JsonProperty("taPosition")]
        public int Position { get; set; }

        [JsonProperty("taParentId")]
        public int ParentId { get; set; }

        [JsonProperty("taComment")]
        public string Comment { get; set; }

        [JsonProperty("taIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("taSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("taAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("taModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("taLastViewDate")]
        public DateTime? LastViewDate { get; set; }
        
        [Ignore]
        public List<UniteItem> UniteItems { get; set; }

        private string name;
        [Ignore]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string _value;
        [Ignore]
        public string Value { get => _value; set => SetProperty(ref _value, value); }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }

        public bool IsToSync { get; set; }

        public Unite()
        {
        }

        public Unite(UniteResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            TypeStr = response.TypeStr;
            Nom = response.Nom;
            IsAlwaysVisible = response.IsAlwaysVisible;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
            FilialeServerKey = response.FilialeServerKey;
            FieldType = response.FieldType;
            Position = response.Position;
            ParentId = response.ParentId;
        }

        public void UpdateFromResponse(UniteResponse response)
        {
            TypeStr = response.TypeStr;
            Nom = response.Nom;
            IsAlwaysVisible = response.IsAlwaysVisible;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
            FilialeServerKey = response.FilialeServerKey;
            FieldType = response.FieldType;
            Position = response.Position;
            ParentId = response.ParentId;
        }
    }
}
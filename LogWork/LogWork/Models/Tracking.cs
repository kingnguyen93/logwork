using Newtonsoft.Json;
using SQLite;
using System;

namespace LogWork.Models
{
    [Table("Tracking")]
    public class Tracking
    {
        [JsonProperty("tr_id")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("tr_user_id")]
        public int UserId { get; set; }

        [JsonProperty("tr_lat")]
        public double Latitude { get; set; }

        [JsonProperty("tr_lon")]
        public double Longitude { get; set; }

        [JsonProperty("tr_alt")]
        public double Altitude { get; set; }

        [JsonProperty("tr_type")]
        public string Type { get; set; }

        [JsonProperty("tr_item_id")]
        public int ItemId { get; set; }

        [JsonProperty("tr_nonce")]
        public string Nonce { get; set; }

        [JsonProperty("tr_date")]
        public DateTime? Date { get; set; }
        
        [JsonIgnore]
        public int IsActif { get; set; }
        
        [JsonIgnore]
        public bool IsToSync { get; set; }
    }
}
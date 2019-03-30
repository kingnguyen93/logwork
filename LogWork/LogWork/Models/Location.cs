using Newtonsoft.Json;
using SQLite;
using System;

namespace LogWork.Models
{
    [Table("Location")]
    public class Location
    {
        [JsonIgnore]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonProperty("llLat")]
        public double Latitude { get; set; }

        [JsonProperty("llLong")]
        public double Longitude { get; set; }

        [JsonProperty("llAlt")]
        public double Altitude { get; set; }

        [JsonProperty("llDate")]
        public DateTime? EditDate { get; set; }
        
        [JsonIgnore]
        public bool IsToSync { get; set; }
    }
}
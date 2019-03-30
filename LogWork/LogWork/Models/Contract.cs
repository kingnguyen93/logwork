using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Contract")]
    public class Contract : TinyModel
    {
        [JsonProperty("conId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("conServerId")]
        public int ServerId { get; set; }

        [JsonProperty("conUserId")]
        public int UserId { get; set; }

        [JsonProperty("conCodeId")]
        public int CodeId { get; set; }

        [JsonProperty("conClientServerId")]
        public int ClientId { get; set; }

        [JsonProperty("conAdresseServerId")]
        public int AdresseId { get; set; }

        [JsonProperty("conCode")]
        public string ConCode { get; set; }

        [JsonProperty("conTitle")]
        public string Title { get; set; }

        [JsonProperty("conDateStart")]
        public DateTime? DateStart { get; set; } //contract date start

        [JsonProperty("conDateEnd")]
        public DateTime? DateEnd { get; set; }

        [JsonProperty("conMinute")]
        public int Minute { get; set; }

        [JsonProperty("conBudget")]
        public decimal Budget { get; set; }

        [JsonProperty("conIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("conSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("conAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("conModifOn")]
        public DateTime? EditDate { get; set; }

        public bool IsToSync { get; set; }

        public Contract()
        {
        }

        public Contract(ContractResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();

            ServerId = response.ServerId;
            CodeId = response.CodeId;
            ConCode = response.ConCode;
            Title = response.Title;
            ClientId = response.FkClientServerId;
            AdresseId = response.FkAddressServerId;
            DateStart = response.ConDateStart;
            DateEnd = response.ConDateEnd;
            Minute = response.ConMinute;
            Budget = response.ConBudget;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }

        public void UpdateFromResponse(ContractResponse response)
        {
            ServerId = response.ServerId;
            CodeId = response.CodeId;
            ConCode = response.ConCode;
            ClientId = response.FkClientServerId;
            AdresseId = response.FkAddressServerId;
            DateStart = response.ConDateStart;
            DateEnd = response.ConDateEnd;
            Minute = response.ConMinute;
            Budget = response.ConBudget;
            IsActif = response.IsActif;
            Title = response.Title;
        }
    }
}
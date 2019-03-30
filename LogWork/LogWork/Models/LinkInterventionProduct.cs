using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("LinkInterventionProduct")]
    public class LinkInterventionProduct : TinyModel
    {
        [JsonProperty("lipId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("lipIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("lipUserId")]
        public int UserId { get; set; }

        [JsonProperty("lipFkProductUUID")]
        public Guid FkProductId { get; set; }

        [JsonProperty("lipFkProductId")]
        public int FkProductServerId { get; set; }

        [JsonProperty("lipFkIntAppliId")]
        public Guid FkInterventionAppliId { get; set; }

        [JsonProperty("lipFkIntId")]
        public int FkInterventionServerId { get; set; }

        [JsonProperty("lipQty")]
        public decimal? Quantity { get; set; }

        [JsonProperty("lipPosition")]
        public int Position { get; set; }

        [JsonProperty("lipPriceNotTax")]
        public decimal? PriceNotTax { get; set; }

        [JsonProperty("lipPriceTax")]
        public decimal? PriceTax { get; set; }

        [JsonProperty("lipProductName")]
        public string ProductName { get; set; }

        [JsonProperty("lipIsPaid")]
        public int IsPaid { get; set; }

        [JsonProperty("lipAmountPaid")]
        public decimal? AmountPaid { get; set; }

        [JsonProperty("lipCurrency")]
        public string Currency { get; set; }

        [JsonProperty("lipComment")]
        public string Comment { get; set; }

        private int isActif;

        [JsonProperty("lipOn")]
        public int IsActif { get => isActif; set => SetProperty(ref isActif, value); }

        [JsonProperty("lipSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("lipAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("lipModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("lipLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        [JsonIgnore]
        [Ignore]
        public string MoreInfo => $"{ProductName}{(AppSettings.MobileEnableProduct ? (" (" + (Product?.PriceTax) + "€)") : "")}";

        [Ignore]
        public Product Product { get; set; }

        [JsonIgnore]
        [Ignore]
        public decimal? TotalPrice => (Product?.Price ?? 0) * (Quantity ?? 0);

        [JsonIgnore]
        [Ignore]
        public decimal? TotalPriceWithTax => (Product?.PriceTax ?? 0) * (Quantity ?? 0);

        public bool IsToSync { get; set; }

        public LinkInterventionProduct()
        {
        }

        public LinkInterventionProduct(LinkInterventionProductResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            FkProductServerId = response.FkProductServerId;
            FkInterventionServerId = response.FkInterventionServerId;
            Quantity = response.Quantity;
            Position = response.Position;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
            ProductName = response.ProductName;
        }

        public void UpdateFromResponse(LinkInterventionProductResponse response)
        {
            FkProductServerId = response.FkProductServerId;
            FkInterventionServerId = response.FkInterventionServerId;
            Quantity = response.Quantity;
            Position = response.Position;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
            ProductName = response.ProductName;
        }
    }
}
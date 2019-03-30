using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Product")]
    public class Product : TinyModel
    {
        [JsonProperty("proId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("proServerId")]
        public int ServerId { get; set; }

        [JsonProperty("proUserId")]
        public int UserId { get; set; }

        [JsonProperty("proCodeId")]
        public int CodeId { get; set; }

        [JsonProperty("proCode")]
        public string Code { get; set; }

        [JsonProperty("proCodeComptable")]
        public string CodeComptable { get; set; }

        private string nom;
        [JsonProperty("proNom")]
        public string Nom { get => nom; set => SetProperty(ref nom, value); }

        [JsonProperty("proSupplier")]
        public string Supplier { get; set; }

        [JsonProperty("proComment")]
        public string Comment { get; set; }

        [JsonProperty("proPurchasePuHt")]
        public decimal PurchasePuHt { get; set; }

        [JsonProperty("proPurchasePuTtc")]
        public decimal PurchasePuTtc { get; set; }

        [JsonProperty("proPurchaseTxTax")]
        public decimal PurchaseTxTax { get; set; }

        private decimal price;
        [JsonProperty("proPrice")]
        public decimal Price { get => price; set => SetProperty(ref price, value); }

        private decimal priceTax;

        [JsonProperty("proPriceTax")]
        public decimal PriceTax { get => priceTax; set => SetProperty(ref priceTax, value); }

        private decimal tax;
        [JsonProperty("proTax")]
        public decimal Tax { get => tax; set => SetProperty(ref tax, value); }

        private decimal quantity;
        [JsonProperty("proQty")]
        public decimal Quantity { get => quantity; set => SetProperty(ref quantity, value); }

        [JsonProperty("proQtyMinAlert")]
        public decimal QtyMinAlert { get; set; }

        [JsonProperty("proQtySold")]
        public decimal QtySold { get; set; }

        [JsonProperty("proIsUnlimited")]
        public int IsUnlimited { get; set; }

        [JsonProperty("proIsQtyRestricted")]
        public int IsQtyRestricted { get; set; }

        [JsonProperty("proCurrency")]
        public string Currency { get; set; }

        [JsonProperty("proIsActif")]
        public int IsActif { get; set; }

        [JsonProperty("proSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("proAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("proEditDate")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("proLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        [JsonIgnore]
        [Ignore]
        public string MoreInfo => string.Join(" ", Nom, "(" + PriceTax + Currency + ")");

        [JsonIgnore]
        [Ignore]
        public string FullInfo => string.Join(" ", "#" + CodeId, Nom, "(" + PriceTax + Currency + ")", "(" + Quantity + ")").Replace("()", "");

        public bool IsToSync { get; set; }

        public Product()
        {
        }

        public Product(ProductResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            CodeId = response.CodeId;
            Nom = response.Nom;
            Code = response.Code;
            Supplier = response.Supplier;
            Comment = response.Comment;
            PurchasePuHt = response.PurchasePuHt;
            PurchasePuTtc = response.PurchasePuTtc;
            PurchaseTxTax = response.PurchaseTxTax;
            Price = response.Price;
            PriceTax = response.PriceTax;
            Tax = response.Tax;
            Quantity = response.Qty;
            QtyMinAlert = response.QtyMinAlert;
            QtySold = response.QtySold;
            IsUnlimited = response.IsUnlimited;
            IsQtyRestricted = response.IsQtyRestricted;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }

        public Product(ProductSync response)
        {
            ServerId = response.ServerId;
            CodeId = response.CodeId;
            Nom = response.Nom;
            Code = response.Code;
            CodeComptable = response.CodeComptable;
            Price = response.Price;
            PriceTax = response.PriceTax;
            Tax = response.Tax;
            Comment = response.Comment;
        }

        public Product(ProductSearchResponse response)
        {
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            CodeId = response.CodeId;
            Nom = response.Nom;
            Code = response.Code;
            CodeComptable = response.CodeComptable;
            Price = response.Price;
            PriceTax = response.PriceTax;
            Tax = response.Tax;
            IsUnlimited = response.IsUnlimited;
        }

        public void UpdateFromResponse(ProductResponse response)
        {
            CodeId = response.CodeId;
            Nom = response.Nom;
            Code = response.Code;
            Supplier = response.Supplier;
            Comment = response.Comment;
            PurchasePuHt = response.PurchasePuHt;
            PurchasePuTtc = response.PurchasePuTtc;
            PurchaseTxTax = response.PurchaseTxTax;
            Price = response.Price;
            PriceTax = response.PriceTax;
            Tax = response.Tax;
            Quantity = response.Qty;
            QtyMinAlert = response.QtyMinAlert;
            QtySold = response.QtySold;
            IsUnlimited = response.IsUnlimited;
            IsQtyRestricted = response.IsQtyRestricted;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }

        public void UpdateFromSync(ProductSync response)
        {
            CodeId = response.CodeId;
            Nom = response.Nom;
            Code = response.Code;
            CodeComptable = response.CodeComptable;
            Price = response.Price;
            PriceTax = response.PriceTax;
            Tax = response.Tax;
            Comment = response.Comment;
        }
    }
}
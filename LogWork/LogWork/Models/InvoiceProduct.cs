using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using System.ComponentModel;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("InvoiceProduct")]
    public class InvoiceProduct : TinyModel
    {
        [JsonProperty("ipId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("ipServerId")]
        public int ServerId { get; set; }

        [JsonProperty("ipUserId")]
        public int UserId { get; set; }

        [JsonProperty("ipAccountId")]
        public int AccountId { get; set; }

        [JsonProperty("ipFkInvoiceUUID")]
        public Guid FKInvoiceAppId { get; set; }

        [JsonProperty("ipFkInvoiceServerId")]
        public int FkInvoiceServerId { get; set; }

        [JsonProperty("ipFkProductUUID")]
        public Guid FkProductAppId { get; set; }

        [JsonProperty("ipFkProductServerId")]
        public int FkProductServerId { get; set; }

        private string label;
        [JsonProperty("ipLabel")]
        public string Label { get => label; set => SetProperty(ref label, value); }

        private decimal percentDiscount;
        [JsonProperty("ipPercentDiscount")]
        public decimal PercentDiscount { get => percentDiscount; set => SetProperty(ref percentDiscount, value); }

        private decimal priceUnit;
        [JsonProperty("ipPriceUnit")]
        public decimal PriceUnit { get => priceUnit; set => SetProperty(ref priceUnit, value); }

        private decimal priceUnitWithTax;
        [JsonProperty("ipPriceUnitWithTax")]
        public decimal PriceUnitWithTax { get => priceUnitWithTax; set => SetProperty(ref priceUnitWithTax, value); }

        private decimal totalPrice;
        [JsonProperty("ipTotalPrice")]
        public decimal TotalPrice { get => totalPrice; set => SetProperty(ref totalPrice, value); }

        private decimal totalPriceWithTax;
        [JsonProperty("ipTotalPriceWithTax")]
        public decimal TotalPriceWithTax { get => totalPriceWithTax; set => SetProperty(ref totalPriceWithTax, value); }

        private decimal rateOfTax;
        [JsonProperty("ipRateOfTax")]
        public decimal RateOfTax { get => rateOfTax; set => SetProperty(ref rateOfTax, value); }

        private decimal amountOfTax;
        [JsonProperty("ipAmountOfTax")]
        public decimal AmountOfTax { get => amountOfTax; set => SetProperty(ref amountOfTax, value); }

        private decimal quantity;
        [JsonProperty("ipQty")]
        public decimal Quantity { get => quantity; set => SetProperty(ref quantity, value); }

        [JsonProperty("ipUnit")]
        public string Unit { get; set; }

        [JsonProperty("ipIsTitle")]
        public int IsTitle { get; set; }

        [JsonProperty("ipPosition")]
        public int Position { get; set; }

        [JsonProperty("ipAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("ipEditDate")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("ipOn")]
        public int IsActif { get; set; }

        [Ignore]
        public Product Product { get; set; }

        [Ignore]
        [JsonIgnore]
        public string ProQuantityPrice => Quantity + " x " + PriceUnit + "€";

        [Ignore]
        [JsonIgnore]
        public decimal TotalDiscount => TotalPriceWithTax * (PercentDiscount / 100);

        [Ignore]
        [JsonIgnore]
        public decimal TotalPayment => TotalPriceWithTax - TotalDiscount;

        public bool IsToSync { get; set; }

        public string PropertyIgnore
        {
            get
            {
                return string.Join(",", nameof(IsToSync));
            }
        }

        public InvoiceProduct()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged -= OnPropertyChanged;

            if (e.PropertyName.Equals(nameof(PercentDiscount)))
            {
                OnPropertyChanged(nameof(TotalDiscount), nameof(TotalPayment));
            }
            else if (e.PropertyName.Equals(nameof(TotalPriceWithTax)))
            {
                OnPropertyChanged(nameof(TotalDiscount), nameof(TotalPayment));
            }
            else if (e.PropertyName.Equals(nameof(PriceUnit)))
            {
                OnPropertyChanged(nameof(ProQuantityPrice));
            }

            PropertyChanged += OnPropertyChanged;
        }

        public InvoiceProduct(InvoiceProductResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = DateTime.Now;
            ServerId = response.ServerId;
            FkInvoiceServerId = response.InvoiceServerId;
            FkProductServerId = response.ProductServerId;
            Label = response.Label;
            PercentDiscount = response.PercentDiscount;
            PriceUnit = response.PriceUnit;
            PriceUnitWithTax = response.PriceUnitWithTax;
            TotalPrice = response.TotalPrice;
            TotalPriceWithTax = response.TotalPriceWithTax;
            RateOfTax = response.RateOfTax;
            AmountOfTax = response.AmountOfTax;
            Quantity = response.Quantity;
            Unit = response.Unit;
            IsTitle = response.IsTitle;
            Position = response.Position;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }

        public void UpdateFromResponse(InvoiceProductResponse response)
        {
            FkInvoiceServerId = response.InvoiceServerId;
            FkProductServerId = response.ProductServerId;
            Label = response.Label;
            PercentDiscount = response.PercentDiscount;
            PriceUnit = response.PriceUnit;
            PriceUnitWithTax = response.PriceUnitWithTax;
            TotalPrice = response.TotalPrice;
            TotalPriceWithTax = response.TotalPriceWithTax;
            RateOfTax = response.RateOfTax;
            AmountOfTax = response.AmountOfTax;
            Quantity = response.Quantity;
            Unit = response.Unit;
            IsTitle = response.IsTitle;
            Position = response.Position;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }
    }
}
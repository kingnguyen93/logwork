using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Invoice")]
    public class Invoice : TinyModel
    {
        [JsonProperty("iId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("iServerId")]
        public int ServerId { get; set; }

        [JsonProperty("iUserId")]
        public int UserId { get; set; }

        [JsonProperty("iAccountId")]
        public int AccountId { get; set; }

        [JsonProperty("iFkClientUUID")]
        public Guid FkClientAppId { get; set; }

        [JsonProperty("iFkClientServerId")]
        public int FkClientServerId { get; set; }

        [JsonProperty("intFkAdresseUUID")]
        public Guid FkAddressAppId { get; set; }

        [JsonProperty("intFkAddressServerId")]
        public int FkAddressServerId { get; set; }

        [JsonProperty("iiFkInterventionUUID")]
        public Guid FkInterventionAppId { get; set; }

        [JsonProperty("iFkInterventionServerId")]
        public int FkInterventionServerId { get; set; }

        [JsonProperty("iFilialeId")]
        public int FilialeId { get; set; }

        [JsonProperty("iIsInvoice")]
        public int IsInvoice { get; set; }

        private string invoiceNumber;
        [JsonProperty("iInvoiceNumber")]
        public string InvoiceNumber { get => invoiceNumber; set => SetProperty(ref invoiceNumber, value); }

        private DateTime? iDate;
        [JsonProperty("iDate")]
        public DateTime? IDate { get => iDate; set => SetProperty(ref iDate, value); }

        private DateTime? iIDueDate;
        [JsonProperty("iIDueDate")]
        public DateTime? IDueDate { get => iIDueDate; set => SetProperty(ref iIDueDate, value); }

        [JsonProperty("iShowDiscountInColumn")]
        public int ShowDiscountInColumn { set; get; }

        [JsonProperty("iReference")]
        public string Reference { get; set; }

        [JsonProperty("iDiscount")]
        public int Discount { set; get; }

        [JsonProperty("iShipping")]
        public int Shipping { set; get; }

        [JsonProperty("iPaymentCondition")]
        public string PaymentCondition { set; get; }

        [JsonProperty("iAmountPaid")]
        public decimal? AmountPaid { set; get; }

        [JsonProperty("iCachePtHt")]
        public decimal? CachePtHt { set; get; }

        [JsonProperty("iCachePtTax")]
        public decimal? CachePtTax { set; get; }

        [JsonProperty("iCachePtTtcToPay")]
        public decimal? CachePtTtcToPay { get; set; }

        [JsonProperty("iPublicComment")]
        public string PublicComment { get; set; }

        [JsonProperty("iPrivateComment")]
        public string PrivateComment { get; set; }

        [JsonProperty("iNonce")]
        public string Nonce { get; set; }

        [JsonProperty("iIsPaid")]
        public int IsPaid { get; set; }

        [JsonProperty("iIsDraft")]
        public int IsDraft { get; set; }

        [JsonProperty("iOn")]
        public int IsActif { get; set; }

        [JsonProperty("iAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("iEditDate")]
        public DateTime? EditDate { get; set; }

        [Ignore]
        [JsonIgnore]
        public string Status => IsDraft == 1 ? TranslateExtension.GetValue("status_in_progress") : IsPaid == 1 ? TranslateExtension.GetValue("status_accepted") : TranslateExtension.GetValue("status_rejected");

        [Ignore]
        [JsonIgnore]
        public int StatusNumber => IsDraft == 1 ? 3 : IsPaid == 1 ? 1 : 5;

        private Client client;
        [Ignore]
        public Client Client { get => client; set => SetProperty(ref client, value); }

        private Address address;
        [Ignore]
        public Address Address { get => address; set => SetProperty(ref address, value); }

        private Intervention intervention;
        [Ignore]
        public Intervention Intervention { get => intervention; set => SetProperty(ref intervention, value); }

        [Ignore]
        public ObservableCollection<InvoiceProduct> LinkInvoiceProducts { get; set; }
        
        public bool IsToSync { get; set; }

        public string PropertyIgnore
        {
            get
            {
                return string.Join(",", nameof(IsToSync));
            }
        }

        public Invoice()
        {
        }

        public Invoice(InvoiceResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = DateTime.Now;
            ServerId = response.ServerId;
            FkClientServerId = response.ClientServerId;
            FkAddressServerId = response.AddressServerId;
            FkInterventionServerId = response.InterventionServerId;
            FilialeId = response.FilialeId;
            IsInvoice = response.IsInvoice;
            InvoiceNumber = response.InvoiceNumber;
            IDate = response.IDate;
            IDueDate = response.IDueDate;
            ShowDiscountInColumn = response.ShowDiscountInColumn;
            Reference = response.Reference;
            Discount = response.Discount;
            Shipping = response.Shipping;
            AmountPaid = response.AmountPaid;
            CachePtHt = response.CachePtHt;
            CachePtTax = response.CachePtTax;
            CachePtTtcToPay = response.CachePtTtcToPay;
            PublicComment = response.PublicComment;
            PrivateComment = response.PrivateComment;
            IsPaid = response.IsPaid;
            IsDraft = response.IsDraft;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }

        public void UpdateFromResponse(InvoiceResponse response)
        {
            FkClientServerId = response.ClientServerId;
            FkAddressServerId = response.AddressServerId;
            FkInterventionServerId = response.InterventionServerId;
            FilialeId = response.FilialeId;
            IsInvoice = response.IsInvoice;
            InvoiceNumber = response.InvoiceNumber;
            IDate = response.IDate;
            IDueDate = response.IDueDate;
            ShowDiscountInColumn = response.ShowDiscountInColumn;
            Reference = response.Reference;
            Discount = response.Discount;
            Shipping = response.Shipping;
            AmountPaid = response.AmountPaid;
            CachePtHt = response.CachePtHt;
            CachePtTax = response.CachePtTax;
            CachePtTtcToPay = response.CachePtTtcToPay;
            PublicComment = response.PublicComment;
            PrivateComment = response.PrivateComment;
            IsPaid = response.IsPaid;
            IsDraft = response.IsDraft;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }
    }
}
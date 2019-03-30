using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class InvoiceResponse
    {
        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int ClientServerId { get; set; }

        [JsonProperty("3")]
        public int AddressServerId { get; set; }

        [JsonProperty("4")]
        public int InterventionServerId { get; set; }

        [JsonProperty("5")]
        public int FilialeId { get; set; }

        [JsonProperty("6")]
        public int IsInvoice { get; set; }

        [JsonProperty("7")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("8")]
        public DateTime? IDate { get; set; }

        [JsonProperty("9")]
        public DateTime? IDueDate { get; set; }

        [JsonProperty("10")]
        public int ShowDiscountInColumn { get; set; }

        [JsonProperty("11")]
        public string Reference { get; set; }

        [JsonProperty("12")]
        public int Discount { get; set; }

        [JsonProperty("13")]
        public int Shipping { get; set; }

        [JsonProperty("14")]
        public decimal? AmountPaid { get; set; }

        [JsonProperty("15")]
        public decimal? CachePtHt { get; set; }

        [JsonProperty("16")]
        public decimal? CachePtTax { get; set; }

        [JsonProperty("17")]
        public decimal? CachePtTtcToPay { get; set; }

        [JsonProperty("18")]
        public string PublicComment { get; set; }

        [JsonProperty("19")]
        public string PrivateComment { get; set; }

        [JsonProperty("20")]
        public int IsPaid { get; set; }

        [JsonProperty("21")]
        public int IsDraft { get; set; }

        [JsonProperty("22")]
        public int IsActif { get; set; }

        [JsonProperty("23")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
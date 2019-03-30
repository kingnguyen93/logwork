using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class InvoiceProductResponse
    {
        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int InvoiceServerId { get; set; }

        [JsonProperty("3")]
        public int ProductServerId { get; set; }

        [JsonProperty("4")]
        public string Label { get; set; }

        [JsonProperty("5")]
        public decimal PercentDiscount { get; set; }

        [JsonProperty("6")]
        public decimal PriceUnit { get; set; }

        [JsonProperty("7")]
        public decimal PriceUnitWithTax { get; set; }

        [JsonProperty("8")]
        public decimal TotalPrice { get; set; }

        [JsonProperty("9")]
        public decimal TotalPriceWithTax { get; set; }

        [JsonProperty("10")]
        public decimal RateOfTax { get; set; }

        [JsonProperty("11")]
        public decimal AmountOfTax { get; set; }

        [JsonProperty("12")]
        public decimal Quantity { get; set; }

        [JsonProperty("13")]
        public string Unit { get; set; }

        [JsonProperty("14")]
        public int IsTitle { get; set; }

        [JsonProperty("15")]
        public int Position { get; set; }

        [JsonProperty("16")]
        public int IsActif { get; set; }

        [JsonProperty("17")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}
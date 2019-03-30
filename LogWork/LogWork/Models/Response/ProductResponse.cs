using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class ProductResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int CodeId { get; set; }

        [JsonProperty("3")]
        public string Nom { get; set; }

        [JsonProperty("4")]
        public string Code { get; set; }

        [JsonProperty("5")]
        public string Supplier { get; set; }

        [JsonProperty("6")]
        public string Comment { get; set; }

        [JsonProperty("7")]
        public decimal PurchasePuHt { get; set; }

        [JsonProperty("8")]
        public decimal PurchasePuTtc { get; set; }

        [JsonProperty("9")]
        public decimal PurchaseTxTax { get; set; }

        [JsonProperty("10")]
        public decimal Price { get; set; }

        [JsonProperty("11")]
        public decimal PriceTax { get; set; }

        [JsonProperty("12")]
        public decimal Tax { get; set; }

        [JsonProperty("13")]
        public decimal Qty { get; set; }

        [JsonProperty("14")]
        public decimal QtyMinAlert { get; set; }

        [JsonProperty("15")]
        public decimal QtySold { get; set; }

        [JsonProperty("16")]
        public int IsUnlimited { get; set; }

        [JsonProperty("17")]
        public int IsQtyRestricted { get; set; }

        [JsonProperty("18")]
        public int IsActif { get; set; }

        [JsonProperty("19")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }

    public class ProductSearchResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int CodeId { get; set; }

        [JsonProperty("3")]
        public string Nom { get; set; }

        [JsonProperty("4")]
        public string Code { get; set; }

        [JsonProperty("5")]
        public string CodeComptable { get; set; }

        [JsonProperty("6")]
        public decimal Price { get; set; }

        [JsonProperty("7")]
        public decimal PriceTax { get; set; }

        [JsonProperty("8")]
        public decimal Tax { get; set; }

        [JsonProperty("9")]
        public int IsUnlimited { get; set; }
    }
}
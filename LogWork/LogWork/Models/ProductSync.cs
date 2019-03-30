using Newtonsoft.Json;
using TinyMVVM;

namespace LogWork.Models
{
    public class ProductSync : TinyModel
    {
        [JsonProperty("pro_id")]
        public int ServerId { get; set; }

        [JsonProperty("pro_code_id")]
        public int CodeId { get; set; }

        [JsonProperty("pro_nom")]
        public string Nom { get; set; }

        [JsonProperty("pro_code")]
        public string Code { get; set; }

        [JsonProperty("pro_code_comptable")]
        public string CodeComptable { get; set; }

        [JsonProperty("pro_sell_pu_ht")]
        public decimal Price { get; set; }

        [JsonProperty("pro_sell_pu_ttc")]
        public decimal PriceTax { get; set; }

        [JsonProperty("pro_sell_tx_tax")]
        public decimal Tax { get; set; }

        [JsonProperty("pro_comment")]
        public string Comment { get; set; }
    }
}
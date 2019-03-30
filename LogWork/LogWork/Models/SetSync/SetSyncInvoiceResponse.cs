using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Forms.Converters;

namespace LogWork.Models.SetSync
{
    public class SetSyncInvoiceResponse
    {
        [JsonProperty("SUCCESS")]
        public string Success { get; set; }

        [JsonProperty("invoices")]
        public List<InvoiceResponse> Invoices { get; set; } = new List<InvoiceResponse>();

        [JsonProperty("invoices_lines")]
        public List<InvoiceLineResponse> InvoicesLines { get; set; } = new List<InvoiceLineResponse>();
    }

    public class InvoiceResponse
    {
        [JsonProperty("appli_id")]
        public string AppId { get; set; }

        [JsonProperty("server_id")]
        public int ServerId { get; set; }

        [JsonProperty("num")]
        public string InvoiceNumber { get; set; }
    }

    public class InvoiceLineResponse
    {
        [JsonProperty("appli_id")]
        public string AppId { get; set; }

        [JsonProperty("server_id")]
        public int ServerId { get; set; }
    }
}
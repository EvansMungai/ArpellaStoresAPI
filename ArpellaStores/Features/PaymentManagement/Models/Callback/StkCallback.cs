using System.Text.Json.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

public class StkCallback
{
    [JsonPropertyName("MerchantRequestID")]
    public string MerchantRequestID { get; set; }

    [JsonPropertyName("CheckoutRequestID")]
    public string CheckoutRequestID { get; set; }

    [JsonPropertyName("ResultCode")]
    public int ResultCode { get; set; }

    [JsonPropertyName("ResultDesc")]
    public string ResultDesc { get; set; }

    [JsonPropertyName("CallbackMetadata")]
    public CallbackMetadata CallbackMetadata { get; set; }
}
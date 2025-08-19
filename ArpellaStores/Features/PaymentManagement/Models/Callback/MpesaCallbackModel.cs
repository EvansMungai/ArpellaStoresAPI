using System.Text.Json.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

public class MpesaCallbackModel
{
    [JsonPropertyName("Body")]
    public CallbackBody Body { get; set; }
}

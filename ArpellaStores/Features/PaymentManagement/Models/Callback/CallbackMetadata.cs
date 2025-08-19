using System.Text.Json.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

public class CallbackMetadata
{
    [JsonPropertyName("Item")]
    public List<CallbackItem> Item { get; set; }
}
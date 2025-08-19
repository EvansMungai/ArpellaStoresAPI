using System.Text.Json.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

public class CallbackItem
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Value")]
    public object Value { get; set; }
}
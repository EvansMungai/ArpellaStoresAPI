using System.Runtime.Serialization;

namespace ArpellaStores.Features.Payment_Management.Models;

/// <summary>
/// Represents a lipa na mpesa query request model
/// </summary>
[DataContract]
public class LipaNaMpesaQueryRequestModel
{
    /// <summary>
    /// This is the shortcode of the organization initiating the request and expecting the payment.
    /// </summary>
    [DataMember(Name = "BusinessShortCode")]
    public int BusinessShortCode { get; set; }

    /// <summary>
    /// This is the Base64-encoded value of the concatenation of the Shortcode + LNM Passkey + Timestamp(YYYYMMDDHHmmss)
    /// </summary>
    [DataMember(Name = "Password")]
    public string Password { get; set; }

    /// <summary>
    /// This is the same Timestamp used in the encoding above, in the format YYYMMDDHHmmss.
    /// </summary>
    [DataMember(Name = "Timestamp")]
    public string Timestamp { get; set; }

    /// <summary>
    /// Check out Request ID
    /// </summary>
    [DataMember(Name = "CheckoutRequestID")]
    public string CheckoutRequestID { get; set; }
}

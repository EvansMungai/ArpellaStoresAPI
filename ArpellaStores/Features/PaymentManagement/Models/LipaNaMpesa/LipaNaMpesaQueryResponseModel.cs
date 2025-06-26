using System.Runtime.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

/// <summary>
/// Represents a lipa na mpesa query response model
/// </summary>
[DataContract]
public class LipaNaMpesaQueryResponseModel
{
    /// <summary>
    /// The merchant request identifier
    /// </summary>
    [DataMember(Name = "MerchantRequestID")]
    public string MerchantRequestID { get; set; }

    /// <summary>
    /// The checkout request identifier
    /// </summary>
    [DataMember(Name = "CheckoutRequestID")]
    public string CheckoutRequestID { get; set; }

    /// <summary>
    /// The response code
    /// </summary>
    [DataMember(Name = "ResponseCode")]
    public string ResponseCode { get; set; }

    /// <summary>
    /// The response description
    /// </summary>
    [DataMember(Name = "ResponseDescription")]
    public string ResponseDescription { get; set; }

    /// <summary>
    /// The result code
    /// </summary>
    [DataMember(Name = "ResultCode")]
    public string ResultCode { get; set; }

    /// <summary>
    /// The desult description
    /// </summary>
    [DataMember(Name = "ResultDesc")]
    public string ResultDesc { get; set; }
}

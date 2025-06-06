using System.Runtime.Serialization;

namespace ArpellaStores.Features.Payment_Management.Models;

/// <summary>
/// Represents a lipa na mpesa response model
/// </summary>
[DataContract]
public class LipaNaMpesaResponseModel
{
    /// <summary>
    /// Merchant Request ID
    /// </summary>
    [DataMember(Name = "MerchantRequestID")]
    public string MerchMerchantRequestID { get; set; }

    /// <summary>
    /// Check out Request ID
    /// </summary>
    [DataMember(Name = "CheckoutRequestID")]
    public string CheckoutRequestID { get; set; }

    /// <summary>
    /// Response Code
    /// </summary>
    [DataMember(Name = "ResponseCode")]
    public string ResponseCode { get; set; }

    /// <summary>
    /// Response Description message
    /// </summary>
    [DataMember(Name = "ResponseDescription")]
    public string ResponseDescription { get; set; }

    /// <summary>
    /// Customer message - Description
    /// </summary>
    [DataMember(Name = "CustomerMessage")]
    public string CustomerMessage { get; set; }
}

using System.Runtime.Serialization;

namespace ArpellaStores.Features.Payment_Management.Models;

/// <summary>
/// Represents a dynamic qr request
/// </summary>
[DataContract]
public class DynamicQrRequestModel
{
    /// <summary>
    /// Gets or sets the transaction code
    /// </summary>
    [DataMember(Name ="TrxCode")]
    public string TransactionCode { get; set; }

    /// <summary>
    /// Gets or sets the credit party identifier
    /// </summary>
    [DataMember(Name = "CPI")]
    public string CreditPartyIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the merchant name
    /// </summary>
    [DataMember(Name = "MerchantName")]
    public string MerchantName { get; set; }

    /// <summary>
    /// Gets or sets the amount
    /// </summary>
    [DataMember(Name = "Amount")]
    public string Amount { get; set; }

    /// <summary>
    /// Gets or sets the reference number
    /// </summary>
    [DataMember(Name = "RefNo")]
    public string ReferenceNumber { get; set; }
}

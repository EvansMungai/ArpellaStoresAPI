using System.Runtime.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

/// <summary>
/// Lipa na mpesa request model
/// </summary>
[DataContract]
public class LipaNaMpesaRequestModel
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
    /// The type of transaction being performed.
    /// These are the same values as the C2B command IDs (CustomerPayBillOnline and CustomerBuyGoodsOnline)
    /// and the same rules apply here. For now, only CustomerPayBillOnline is supported.
    /// </summary>
    [DataMember(Name = "TransactionType")]
    public string TransactionType { get; set; }

    /// <summary>
    /// The amount being transacted
    /// </summary>
    [DataMember(Name = "Amount")]
    public int Amount { get; set; }

    /// <summary>
    /// The Debit party of the transaction/the party paying out in the transaction, hereby the phone number of the customer.
    /// </summary>
    [DataMember(Name = "PartyA")]
    public long PartyA { get; set; }

    /// <summary>
    /// The credit party of the transaction/the party being paid in the transaction,
    /// hereby being the shortcode of the organization. This is the same value as the Business Shortcode
    /// </summary>
    [DataMember(Name = "PartyB")]
    public int PartyB { get; set; }

    /// <summary>
    /// Same as PartyA.
    /// </summary>
    [DataMember(Name = "PhoneNumber")]
    public long PhoneNumber { get; set; }

    /// <summary>
    /// This is the endpoint where you want the results of the transaction delivered. Same rules for Register URL API callbacks apply
    /// </summary>
    [DataMember(Name = "CallBackURL")]
    public string CallBackUrl { get; set; }

    /// <summary>
    /// This is the value the customer would have put as the account number on their phone if they had performed the transaction via phone.
    /// </summary>
    [DataMember(Name = "AccountReference")]
    public string AccountReference { get; set; }

    /// <summary>
    /// Short description of the transaction. Optional, but element must be present.
    /// </summary>
    [DataMember(Name = "TransactionDesc")]
    public string TransactionDescription { get; set; }
}

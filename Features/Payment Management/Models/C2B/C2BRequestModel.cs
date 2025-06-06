using System.Runtime.Serialization;

namespace ArpellaStores.Features.Payment_Management.Models;

/// <summary>
/// Represents a customer to business mpesa payment request model
/// </summary>
[DataContract]
public class C2BRequestModel
{
    /// <summary>
    /// This is your paybill number/till number, which you expect to receive payments notifications about.
    /// </summary>
    [DataMember(Name = "ShortCode")]
    public int ShortCode { get; set; }

    /// <summary>
    /// This is a transaction type identifier, which Identifies the type of C2B transaction being made. 
    /// There are two options: CustomerPayBillOnline and CustomerBuyGoodsOnline. 
    /// </summary>
    [DataMember(Name = "CommandID")]
    public string CommandId { get; set; }

    /// <summary>
    /// The amount to be transacted
    /// </summary>
    [DataMember(Name = "Amount")]
    public long Amount { get; set; }

    /// <summary>
    /// This is the test phone number of the virtual customer paying to your paybill.
    /// </summary>
    [DataMember(Name = "Msisdn")]
    public long Msisdn { get; set; }

    /// <summary>
    /// This simulates the account number that a user would have entered when making a Pay Bill request. 
    /// This parameter is only required for CustomerPayBillOnline transaction type.
    /// It has a maximum of 20 characters.
    /// </summary>
    [DataMember(Name = "BillRefNumber")]
    public string BillRefNumber { get; set; }
}

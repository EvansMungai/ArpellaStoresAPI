using System.Runtime.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

/// <summary>
/// Represents the request model to register the callback URLs 
/// via which you shall receive payment notifications for payments to your paybill/till number
/// </summary>
[DataContract]
public class RegisterUrlRequestModel
{
    /// <summary>
    /// This is your C2B-enabled paybill number/till number, which you expect to receive payments notifications about.
    /// </summary>
    [DataMember(Name = "ShortCode")]
    public int ShortCode { get; set; }

    /// <summary>
    /// This is the default action value that determines what M-Pesa will do in the scenario that your Validation endpoint is unreachable or is unable to respond on time.
    /// Only two values are allowed: Completed or Cancelled. 
    /// Completed means M-Pesa will automatically complete your transaction, 
    /// whereas Cancelled means M-Pesa will automatically cancel the transaction.
    /// </summary>
    [DataMember(Name = "ResponseType")]
    public string ResponseType { get; set; }

    /// <summary>
    /// Confirmation URL
    /// </summary>
    [DataMember(Name = "ConfirmationURL")]
    public string ConfirmationUrl { get; set; }

    /// <summary>
    /// Validation URL
    /// </summary>
    [DataMember(Name = "ValidationURL")]
    public string ValidationUrl { get; set; }
}

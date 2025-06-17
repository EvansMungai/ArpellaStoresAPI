using System.Runtime.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

/// <summary>
/// Represents a business to customer request model
/// </summary>
[DataContract]
public class B2CRequestModel
{
    /// <summary>
    /// The username of the API operator as assigned on the M-Pesa Org Portal.
    /// </summary>
    [DataMember(Name = "InitiatorName")]
    public string InitiatorName { get; set; }

    /// <summary>
    /// The password of the API operator encrypted using the public key certificate provided.
    /// Write the unencrypted password into a byte array.
    /// Encrypt the array with the M-Pesa public key certificate. 
    /// Use the RSA algorithm, and use PKCS #1.5 padding (not OAEP), and add the result to the encrypted stream.
    /// Convert the resulting encrypted byte array into a string using base64 encoding. The resulting base64 encoded string is the security credential.
    /// </summary>
    [DataMember(Name = "SecurityCredential")]
    public string SecurityCredential { get; set; }

    /// <summary>
    /// This specifies the type of transaction being performed. 
    /// There are three allowed values on the API: SalaryPayment, BusinessPayment or PromotionPayment.
    /// </summary>
    [DataMember(Name = "CommandID")]
    public string CommandID { get; set; }

    /// <summary>
    /// The amount being transacted
    /// </summary>
    [DataMember(Name = "Amount")]
    public long Amount { get; set; }

    /// <summary>
    /// This is the identifier of the Debit party of the transaction, 
    /// in this case the debit party being the Organization thus the identifier being the Shortcode of the organization.
    /// </summary>
    [DataMember(Name = "PartyA")]
    public long PartyA { get; set; }

    /// <summary>
    /// This is the identifier of the Credit party of the transaction, 
    /// here the credit party being the customer thus the identifier being the customer's phone number (beginning with 07XX or 2547XX)
    /// </summary>
    [DataMember(Name = "PartyB")]
    public long PartyB { get; set; }

    /// <summary>
    /// A very short description of the transaction from your end. Cannot be blank.
    /// </summary>
    [DataMember(Name = "Remarks")]
    public string Remarks { get; set; }

    /// <summary>
    /// This is the callback URL used to send an error callback when the transaction was not able to be processed by M-Pesa within a stipulated time period.
    /// </summary>
    [DataMember(Name = "QueueTimeOutURL")]
    public string QueueTimeOutURL { get; set; }

    /// <summary>
    /// This is the callback URL where the results of the transaction will be sent. 
    /// Please visit the API Apps section to understand how this is used if you are not familiar with it.
    /// </summary>
    [DataMember(Name = "ResultURL")]
    public string ResultURL { get; set; }

    /// <summary>
    /// A very short description of the transaction from your end. Can be blank
    /// </summary>
    [DataMember(Name = "Occassion")]
    public string Occassion { get; set; }
}

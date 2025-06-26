using System.Runtime.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

/// <summary>
/// Represents a Reversal request model
/// </summary>
[DataContract]
public class ReversalRequestModel
{
    /// <summary>
    /// The username of the API operator as assigned on the M-Pesa Org Portal.
    /// </summary>
    [DataMember(Name = "Initiator")]
    public string Initiator { get; set; }

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
    /// TransactionReversal
    /// </summary>
    [DataMember(Name = "CommandID")]
    public string CommandID { get; set; }

    /// <summary>
    /// This is the M-Pesa Transaction ID of the transaction which you wish to reverse.
    /// </summary>
    [DataMember(Name = "TransactionID")]
    public string TransactionID { get; set; }

    /// <summary>
    /// Your Org's shortcode here.
    /// </summary>
    [DataMember(Name = "ReceiverParty")]
    public long ReceiverParty { get; set; }

    /// <summary>
    /// The identity type 4
    /// </summary>
    [DataMember(Name = "RecieverIdentifierType")]
    public string RecieverIdentifierType { get; set; }

    /// <summary>
    /// This is the callback URL where the results of the transaction will be sent. 
    /// Please visit the API Apps section to understand how this is used if you are not familiar with it.
    /// </summary>
    [DataMember(Name = "ResultURL")]
    public string ResultURL { get; set; }

    /// <summary>
    /// This is the callback URL used to send an error callback when the transaction was not able to be processed by M-Pesa within a stipulated time period.
    /// </summary>
    [DataMember(Name = "QueueTimeOutURL")]
    public string QueueTimeOutURL { get; set; }

    /// <summary>
    /// A very short description of the transaction from your end. Cannot be blank.
    /// </summary>
    [DataMember(Name = "Remarks")]
    public string Remarks { get; set; }

    /// <summary>
    /// A very short description of the transaction from your end. Can be blank
    /// </summary>
    [DataMember(Name = "Occasion")]
    public string Occasion { get; set; }
}

using System.Runtime.Serialization;

namespace ArpellaStores.Features.Payment_Management.Models;

/// <summary>
/// Represents a dynamic qr response
/// </summary>
[DataContract]
public class DynamicQrResponseModel
{
    /// <summary>
    /// Gets or sets the response code
    /// </summary>
    [DataMember(Name = "ResponseCode")]
    public string ResponseCode { get; set; }


    /// <summary>
    /// Gets or sets the response description
    /// </summary>
    [DataMember(Name = "ResponseDescription")]
    public string ResponseDescription { get; set; }

    /// <summary>
    /// Gets or sets the qr code
    /// </summary>
    [DataMember(Name = "QrCode")]
    public string QrCode { get; set; }
}

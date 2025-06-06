using System.Runtime.Serialization;

namespace ArpellaStores.Features.Payment_Management.Models;

/// <summary>
/// Represents a business to customer response
/// </summary>
[DataContract]
public class B2CResponseModel : CommonResponseModel
{

    /// <summary>
    /// The response code
    /// </summary>
    [DataMember(Name = "ResponseCode")]
    public string ResponseCode { get; set; }
}

using System.Runtime.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

/// <summary>
/// Represents the oauth response model
/// </summary>
[DataContract]
public class OAuthResponseModel
{
    /// <summary>
    /// Access token
    /// </summary>
    [DataMember(Name = "access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Expiry perios remaining
    /// </summary>
    [DataMember(Name = "expires_in")]
    public string ExpiresIn { get; set; }
}

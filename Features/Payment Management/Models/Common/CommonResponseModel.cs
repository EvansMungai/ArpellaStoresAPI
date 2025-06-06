using System.Runtime.Serialization;

namespace ArpellaStores.Features.Payment_Management.Models;

/// <summary>
/// Represents a reversal response model
/// </summary>
[DataContract]
public class CommonResponseModel
{
    /// <summary>
    /// Gets or sets the originator conversation ID
    /// </summary>
    [DataMember(Name = "OriginatorConversationID")]
    public string OriginatorConversationId { get; set; }

    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    [DataMember(Name = "ConversationID")]
    public string ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the response description
    /// </summary>
    [DataMember(Name = "ResponseDescription")]
    public string ResponseDescription { get; set; }
}

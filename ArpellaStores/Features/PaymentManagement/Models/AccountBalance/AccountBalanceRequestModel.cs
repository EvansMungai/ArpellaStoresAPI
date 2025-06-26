using System.Runtime.Serialization;

namespace ArpellaStores.Features.PaymentManagement.Models;

[DataContract]
public class AccountBalanceRequestModel
{
    [DataMember(Name = "CommandID")]
    public string CommandId { get; set; }

    [DataMember(Name = "PartyA")]
    public long PartyA { get; set; }

    [DataMember(Name = "IdentifierType")]
    public long IdentifierType { get; set; }

    [DataMember(Name = "Remarks")]
    public string Remarks { get; set; }

    [DataMember(Name = "Initiator")]
    public string Initiator { get; set; }

    [DataMember(Name = "SecurityCredential")]
    public string SecurityCredential { get; set; }

    [DataMember(Name = "QueueTimeOutURL")]
    public string QueueTimeOutURL { get; set; }

    [DataMember(Name = "ResultURL")]
    public string ResultUrl { get; set; }
}


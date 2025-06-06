namespace ArpellaStores.Features.Payment_Management.Common;

public class MpesaConfig
{
    public string ConsumerKey { get; set; }
    public string ConsumerSecret { get; set; }
    public string BusinessShortCode { get; set; }
    public string Passkey { get; set; }
    public string CallbackUri { get; set; }
    public string ConfirmationUri { get; set; }
    public string ValidationUri { get; set; }
    public string BaseUri { get; set; }
}

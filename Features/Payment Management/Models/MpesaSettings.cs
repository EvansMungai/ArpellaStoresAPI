namespace ArpellaStores.Features.Payment_Management.Models;

public class MpesaSettings
{
    public string ConsumerKey { get; set; }
    public string ConsumerSecret { get; set; }
    public string ShortCode { get; set; }
    public string Passkey { get; set; }
    public string CallbackUrl { get; set; }
}

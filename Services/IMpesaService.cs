using ArpellaStores.Models;

namespace ArpellaStores.Services;

public interface IMpesaService
{
    Task<string> GetToken();
    Task<string> RegisterUrls();
    Task<string> SendPaymentPrompt(MpesaExpress mpesa);
}

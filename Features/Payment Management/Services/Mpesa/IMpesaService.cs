using ArpellaStores.Features.Payment_Management.Models;

namespace ArpellaStores.Services;

public interface IMpesaService
{
    Task<string> GetToken();
    Task<string> RegisterUrls();
    Task<string> SendPaymentPrompt(MpesaExpressRequest mpesa);
}

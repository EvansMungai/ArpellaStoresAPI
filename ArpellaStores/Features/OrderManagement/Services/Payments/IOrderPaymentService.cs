using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.PaymentManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderPaymentService
{
    Task<LipaNaMpesaResponseModel> InitiateStkPushAsync(CachedOrderDto order);
}

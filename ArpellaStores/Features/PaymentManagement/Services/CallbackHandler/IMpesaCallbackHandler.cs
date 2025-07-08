using ArpellaStores.Features.PaymentManagement.Models;

namespace ArpellaStores.Features.PaymentManagement.Services;

public interface IMpesaCallbackHandler
{
    Task<IResult> HandleAsync(MpesaCallbackModel callback);
}

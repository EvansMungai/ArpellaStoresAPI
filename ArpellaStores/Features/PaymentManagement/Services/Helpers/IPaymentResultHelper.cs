namespace ArpellaStores.Features.PaymentManagement.Services;

public interface IPaymentResultHelper
{
    Task<IResult> GetPaymentStatusAsync(string checkoutRequestId);
}

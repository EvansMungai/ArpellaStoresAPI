using Microsoft.Extensions.Caching.Memory;

namespace ArpellaStores.Features.PaymentManagement.Services;

public class PaymentResultHelper : IPaymentResultHelper
{
    private readonly IMemoryCache _cache;
    public PaymentResultHelper(IMemoryCache cache)
    {
        _cache = cache;
    }
    public async Task<IResult> GetPaymentStatusAsync(string checkoutRequestId)
    {
        string cacheKey = $"payment-result-{checkoutRequestId}";

        if (_cache.TryGetValue<object>(cacheKey, out var resultData))
        {
            return Results.Ok(resultData);
        }

        return Results.Ok(new
        {
            Status = "Pending",
            Message = "Payment is still being processed or has expired."
        });
    }
}

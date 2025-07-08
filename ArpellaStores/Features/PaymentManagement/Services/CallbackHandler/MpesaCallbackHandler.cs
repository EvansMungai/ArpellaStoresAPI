using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.OrderManagement.Services;
using ArpellaStores.Features.PaymentManagement.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ArpellaStores.Features.PaymentManagement.Services;

public class MpesaCallbackHandler : IMpesaCallbackHandler
{
    private readonly IMemoryCache _cache;
    private readonly ArpellaContext _context;
    private readonly IMpesaApiService _mpesaApi;
    private readonly IOrderRepository _repo;
    public MpesaCallbackHandler(IMemoryCache cache, ArpellaContext context, IMpesaApiService mpesaApi, IOrderRepository repo)
    {
        _cache = cache;
        _context = context;
        _mpesaApi = mpesaApi;
        _repo = repo;
    }

    public async Task<IResult> HandleAsync(MpesaCallbackModel callback)
    {
        var stk = callback.Body?.stkCallback;
        var metadata = stk?.CallbackMetadata?.Item;
        if (stk == null || metadata == null)
            return Results.BadRequest("Invalid callback structure.");

        if (stk.ResultCode != 0)
        {
            _cache.Remove($"pending-order-{stk.CheckoutRequestID}");
            return Results.BadRequest($"Payment failed: {stk.ResultDesc}");
        }

        string cacheKey = $"pending-order-{stk.CheckoutRequestID}";
        if (!_cache.TryGetValue<Order>(cacheKey, out var cachedOrder))
            return Results.BadRequest("No pending order found.");

        string transactionId = _mpesaApi.GetValue(metadata, "MpesaReceiptNumber");

        try
        {
            await _repo.FinalizeOrderAsync(cachedOrder, transactionId);
            _cache.Remove(cacheKey);
            return Results.Ok("Order successfully recorded after confirmed payment.");
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Persistence error: {ex.Message}");
        }
    }
}

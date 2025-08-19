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
    private readonly IOrderHelper _helper;
    public MpesaCallbackHandler(IMemoryCache cache, ArpellaContext context, IMpesaApiService mpesaApi, IOrderRepository repo, IOrderHelper helper)
    {
        _cache = cache;
        _context = context;
        _mpesaApi = mpesaApi;
        _repo = repo;
        _helper = helper;
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
            _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
            {
                Status = "Failed",
                Description = stk.ResultDesc
            }, TimeSpan.FromMinutes(10));

            return Results.BadRequest($"Payment failed: {stk.ResultDesc}");
        }

        string cacheKey = $"pending-order-{stk.CheckoutRequestID}";
        if (!_cache.TryGetValue<Order>(cacheKey, out var cachedOrder))
            return Results.BadRequest("No pending order found.");

        string transactionId = _mpesaApi.GetValue(metadata, "MpesaReceiptNumber");
        //if (string.IsNullOrEmpty(transactionId))
        //    return Results.BadRequest("Missing MpesaReceiptNumber in callback.");


        try
        {
            var rebuiltOrder = _helper.RebuildOrder(cachedOrder);
            await _repo.FinalizeOrderAsync(rebuiltOrder, transactionId);

            _cache.Remove(cacheKey);
            _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
            {
                Status = "Success",
                Description = stk.ResultDesc,
                OrderId = cachedOrder.Orderid
            }, TimeSpan.FromMinutes(10));

            return Results.Ok(new
            {
                Message = "Order successfully recorded after confirmed payment.",
                OrderId = cachedOrder.Orderid,
                TransactionId = transactionId,
                Amount = cachedOrder.Total,
                PhoneNumber = cachedOrder.PhoneNumber
            });
        }
        catch (Exception ex)
        {
            _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
            {
                Status = "Error",
                Message = ex.Message
            }, TimeSpan.FromMinutes(10));
            return Results.BadRequest($"Persistence error: {ex.Message}");
        }
    }
}

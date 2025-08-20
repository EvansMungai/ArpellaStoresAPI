using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.OrderManagement.Services;
using ArpellaStores.Features.PaymentManagement.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ArpellaStores.Features.PaymentManagement.Services;

public class MpesaCallbackHandler : IMpesaCallbackHandler
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MpesaCallbackHandler> _logger;
    private readonly IMpesaApiService _mpesaApi;
    private readonly IOrderRepository _repo;
    private readonly IOrderHelper _helper;
    public MpesaCallbackHandler(IMemoryCache cache, ILogger<MpesaCallbackHandler> logger, IMpesaApiService mpesaApi, IOrderRepository repo, IOrderHelper helper)
    {
        _cache = cache;
        _logger = logger;
        _mpesaApi = mpesaApi;
        _repo = repo;
        _helper = helper;
    }

    public async Task<IResult> HandleAsync(HttpRequest request)
    {
        try
        {
            using var reader = new StreamReader(request.Body);
            var rawBody = await reader.ReadToEndAsync();
            _logger.LogInformation($"Received callback: {rawBody}");

            MpesaCallbackModel? callback;
            try
            {
                callback = JsonSerializer.Deserialize<MpesaCallbackModel>(rawBody);
            }
            catch (Exception ex)
            {
                return Results.BadRequest("Malformed JSON payload");
            }

            var stk = callback.Body?.stkCallback;
            var metadata = stk?.CallbackMetadata?.Item;
            if (stk == null || metadata == null)
                return Results.BadRequest("Invalid callback structure.");

            if (stk.ResultCode != 0)
            {
                _logger.LogInformation("Payment Transaction failed.");
                _cache.Remove($"pending-order-{stk.CheckoutRequestID}");
                _logger.LogInformation("Cached Order removed");
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Failed",
                    Description = stk.ResultDesc
                }, TimeSpan.FromMinutes(10));
                _logger.LogInformation("Cached failed payment transaction");
                return Results.BadRequest($"Payment failed: {stk.ResultDesc}");
            }

            _logger.LogInformation("Payment Transaction successful");
            string cacheKey = $"pending-order-{stk.CheckoutRequestID}";
            if (!_cache.TryGetValue<Order>(cacheKey, out var cachedOrder))
            {
                _logger.LogInformation("Cached order was not found after successful transaction");
                return Results.BadRequest("No pending order found.");
            }


            string transactionId = _mpesaApi.GetValue(metadata, "MpesaReceiptNumber");
            if (string.IsNullOrEmpty(transactionId))
            {
                _logger.LogInformation("Missing receipt number after successful payment");
                return Results.BadRequest("Missing MpesaReceiptNumber in callback.");
            }


            try
            {
                _logger.LogInformation("Building cached order for storing it.");
                var rebuiltOrder = _helper.RebuildOrder(cachedOrder);
                await _repo.FinalizeOrderAsync(rebuiltOrder, transactionId);
                _logger.LogInformation("Finished storing order details");

                _cache.Remove(cacheKey);
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Success",
                    Description = stk.ResultDesc,
                    OrderId = cachedOrder.Orderid
                }, TimeSpan.FromMinutes(10));

                return Results.Ok(new MpesaCallbackResponse
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
        catch (Exception ex) { return Results.Problem($"Internal Server Error: {ex.Message}"); }
    }
}

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
    private readonly IOrderHelper _helper;
    private readonly IOrderFinalizerService _orderFinalizerService;
    public MpesaCallbackHandler(IMemoryCache cache, ILogger<MpesaCallbackHandler> logger, IMpesaApiService mpesaApi, IOrderHelper helper, IOrderFinalizerService orderFinalizerService)
    {
        _cache = cache;
        _logger = logger;
        _mpesaApi = mpesaApi;
        _helper = helper;
        _orderFinalizerService = orderFinalizerService;
    }

    public async Task<IResult> HandleAsync(HttpRequest request)
    {
        try
        {
            using var reader = new StreamReader(request.Body);
            var rawBody = await reader.ReadToEndAsync();

            MpesaCallbackModel? callback;
            try
            {
                callback = JsonSerializer.Deserialize<MpesaCallbackModel>(rawBody);
            }
            catch (Exception ex)
            {
                return Results.Json(new { status = "error", message = "Malformed JSON payload" }, statusCode: 400);
            }

            var stk = callback?.Body?.stkCallback;
            var metadata = stk?.CallbackMetadata?.Item;

            if (stk == null)
                return Results.Json(new { status = "error", message = "Missing stkCallback" }, statusCode: 400);

            if (stk.ResultCode != 0)
            {
                _logger.LogInformation($"Payment failed: {stk.ResultDesc}");
                //_cache.Remove($"pending-order-{stk.CheckoutRequestID}");
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Failed",
                    Description = stk.ResultDesc
                }, TimeSpan.FromMinutes(10));

                return Results.Json(new
                {
                    status = "failed",
                    reason = stk.ResultDesc,
                    code = stk.ResultCode
                }, statusCode: 400);
            }

            if (metadata == null)
                return Results.Json(new { status = "error", message = "Missing CallbackMetadata" }, statusCode: 400);

            string cacheKey = $"pending-order-{stk.CheckoutRequestID}";
            if (!_cache.TryGetValue<CachedOrderDto>(cacheKey, out var cachedOrder))
            {
                _logger.LogWarning("No pending order found.");
                return Results.Json(new { status = "error", message = "No pending order found." }, statusCode: 400);
            }

            string transactionId = _mpesaApi.GetValue(metadata, "MpesaReceiptNumber");
            if (string.IsNullOrEmpty(transactionId))
            {
                _logger.LogWarning("Missing MpesaReceiptNumber.");
                return Results.Json(new { status = "error", message = "Missing MpesaReceiptNumber in callback." }, statusCode: 400);
            }

            try
            {
                _logger.LogInformation("Finalizing order...");
                var rebuiltOrder = _helper.RebuildOrder(cachedOrder);
                await _orderFinalizerService.FinalizeOrderAsync(rebuiltOrder, transactionId);

                _cache.Remove(cacheKey);
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Success",
                    Description = stk.ResultDesc,
                    OrderId = cachedOrder.Orderid
                }, TimeSpan.FromMinutes(10));
                _logger.LogInformation("Payment and saving items was successful.");
                return Results.Json(new 
                {
                    Message = "Order successfully recorded after confirmed payment.",
                    OrderId = cachedOrder.Orderid,
                    TransactionId = transactionId,
                    Amount = cachedOrder.Total,
                    PhoneNumber = cachedOrder.PhoneNumber
                }, statusCode: 200);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Persistence error: {ex.Message}");
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Error",
                    Message = ex.Message
                }, TimeSpan.FromMinutes(10));

                return Results.Json(new { status = "error", message = $"Persistence error: {ex.Message}" }, statusCode: 500);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled exception: {ex.Message}");
            return Results.Problem("Internal Server Error.");
        }
    }
}

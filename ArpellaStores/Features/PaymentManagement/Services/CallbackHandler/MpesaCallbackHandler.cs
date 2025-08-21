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
    private readonly IServiceProvider _serviceProvider;
    public MpesaCallbackHandler(IMemoryCache cache, ILogger<MpesaCallbackHandler> logger, IMpesaApiService mpesaApi, IOrderHelper helper, IServiceProvider serviceProvider)
    {
        _cache = cache;
        _logger = logger;
        _mpesaApi = mpesaApi;
        _helper = helper;
        _serviceProvider = serviceProvider;
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
                return Results.BadRequest(new { status = "error", message = "Malformed JSON payload" });
            }

            var stk = callback?.Body?.stkCallback;
            var metadata = stk?.CallbackMetadata?.Item;

            if (stk == null)
                return Results.BadRequest(new { status = "error", message = "Missing stkCallback" });

            if (stk.ResultCode != 0)
            {
                _logger.LogInformation($"Payment failed: {stk.ResultDesc}");
                //_cache.Remove($"pending-order-{stk.CheckoutRequestID}");
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Failed",
                    Description = stk.ResultDesc
                }, TimeSpan.FromMinutes(10));

                return Results.BadRequest(new
                {
                    status = "failed",
                    reason = stk.ResultDesc,
                    code = stk.ResultCode
                });
            }

            if (metadata == null)
                return Results.BadRequest(new { status = "error", message = "Missing CallbackMetadata" });

            string cacheKey = $"pending-order-{stk.CheckoutRequestID}";
            if (!_cache.TryGetValue<CachedOrderDto>(cacheKey, out var cachedOrder))
            {
                _logger.LogWarning("No pending order found.");
                return Results.NotFound(new { status = "error", message = "No pending order found." });
            }

            string transactionId = _mpesaApi.GetValue(metadata, "MpesaReceiptNumber");
            if (string.IsNullOrEmpty(transactionId))
            {
                _logger.LogWarning("Missing MpesaReceiptNumber.");
                return Results.BadRequest(new { status = "error", message = "Missing MpesaReceiptNumber in callback." });
            }

            try
            {
                _logger.LogInformation("Finalizing order...");
                var rebuiltOrder = _helper.RebuildOrder(cachedOrder);

                using var scope = _serviceProvider.CreateScope();
                var finalizer = scope.ServiceProvider.GetRequiredService<IOrderFinalizerService>();
                await finalizer.FinalizeOrderAsync(rebuiltOrder, transactionId);
                

                _cache.Remove(cacheKey);
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Success",
                    Description = stk.ResultDesc,
                    OrderId = cachedOrder.Orderid
                }, TimeSpan.FromMinutes(10));
                _logger.LogInformation("Payment and saving items was successful.");
                return Results.Ok(new {status = "success", message = "Payment processed successfully and order has been saved."});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Persistence error: {ex.Message}");
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Error",
                    Message = ex.Message
                }, TimeSpan.FromMinutes(10));

                return Results.BadRequest(new { status = "error", message = $"Persistence error: {ex.Message}" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled exception: {ex.Message}");
            return Results.Problem("Internal Server Error.");
        }
    }
}

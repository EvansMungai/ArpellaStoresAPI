using ArpellaStores.Features.OrderManagement.Services;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.SmsManagement.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ArpellaStores.Features.PaymentManagement.Services;

public class MpesaCallbackHandler : IMpesaCallbackHandler
{
    private readonly IMemoryCache _cache;
    private readonly IMpesaApiService _mpesaApi;
    private readonly IOrderHelper _helper;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISmsHelpers _smsHelpers;
    public MpesaCallbackHandler(IMemoryCache cache, IMpesaApiService mpesaApi, IOrderHelper helper, IServiceProvider serviceProvider, ISmsHelpers smsHelpers)
    {
        _cache = cache;
        _mpesaApi = mpesaApi;
        _helper = helper;
        _serviceProvider = serviceProvider;
        _smsHelpers = smsHelpers;
    }

    public async Task<IResult> HandleAsync(HttpRequest request)
    {
        try
        {
            using var reader = new StreamReader(request.Body);
            var rawBody = await reader.ReadToEndAsync();

            MpesaCallbackModel? callback;
            try { callback = JsonSerializer.Deserialize<MpesaCallbackModel>(rawBody); }
            catch (Exception ex) { return Results.BadRequest("Malformed JSON payload"); }

            var stk = callback?.Body?.stkCallback;
            var metadata = stk?.CallbackMetadata?.Item;

            if (stk == null)
                return Results.BadRequest("Missing stkCallback");

            if (stk.ResultCode != 0)
            {
                _cache.Remove($"pending-order-{stk.CheckoutRequestID}");
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Failed",
                    Description = stk.ResultDesc
                }, TimeSpan.FromMinutes(10));

                return Results.BadRequest($"STK Failed:  {stk.ResultCode} =  {stk.ResultDesc}");
            }

            if (metadata == null)
                return Results.BadRequest("Missing CallbackMetadata");

            string cacheKey = $"pending-order-{stk.CheckoutRequestID}";
            if (!_cache.TryGetValue<CachedOrderDto>(cacheKey, out var cachedOrder))
                return Results.NotFound(new { status = "error", message = "No pending order found." });

            string transactionId = _mpesaApi.GetValue(metadata, "MpesaReceiptNumber");
            if (string.IsNullOrEmpty(transactionId))
                return Results.BadRequest("Missing MpesaReceiptNumber in callback.");


            try
            {
                var rebuiltOrder = _helper.RebuildOrder(cachedOrder);
                var orderManagerNumbers = await _smsHelpers.GetUsersInRoleAsync("Order Manager");

                using var scope = _serviceProvider.CreateScope();
                var finalizer = scope.ServiceProvider.GetRequiredService<IOrderFinalizerService>();
                await finalizer.FinalizeOrderAsync(rebuiltOrder, transactionId);
                //var notificationService = scope.ServiceProvider.GetRequiredService<IOrderNotificationService>();
                //await notificationService.NofityCustomerAsync(rebuiltOrder);
                //await notificationService.NotifyOrderManagerAsync(rebuiltOrder, orderManagerNumbers);


                _cache.Remove(cacheKey);
                _cache.Set($"payment-result-{stk.CheckoutRequestID}", new
                {
                    Status = "Success",
                    Description = stk.ResultDesc,
                    OrderId = cachedOrder.Orderid
                }, TimeSpan.FromMinutes(10));

                return Results.Ok("Payment processed successfully and order has been saved.");
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
        catch (Exception ex) { return Results.Problem($"Internal Server Error: {ex.InnerException?.Message ?? ex.Message}"); }
    }
}

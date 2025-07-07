using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace ArpellaStores.Features.PaymentManagement.Endpoints;

public class PaymentRoutes : IRouteRegistrar
{
    private readonly IMpesaApiService _mpesaApiService;
    private readonly MpesaConfig _mpesaConfig;
    private readonly ArpellaContext _context;
    private readonly IMemoryCache _cache;

    public PaymentRoutes(IMpesaApiService mpesaApiService, IOptions<MpesaConfig> mpesaConfig, ArpellaContext context, IMemoryCache cache)
    {
        _mpesaApiService = mpesaApiService;
        _mpesaConfig = mpesaConfig.Value;
        _context = context;
        _cache = cache;
    }

    public void RegisterRoutes(WebApplication app)
    {
        MapPaymentRoutes(app);
    }

    public void MapPaymentRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Mpesa");
        app.MapGet("/access-token", (PaymentHandler handler) => handler.GenerateAccessToken());
        app.MapPost("register-url", async (PaymentHandler handler) =>
        {
            string registerUri = "https://api.safaricom.co.ke/mpesa/c2b/v1/registerurl";
            var requestModel = new RegisterUrlRequestModel
            {
                ShortCode = 5142142,
                ResponseType = "Completed",
                ConfirmationUrl = _mpesaConfig.ConfirmationUri,
                ValidationUrl = _mpesaConfig.ValidationUri
            };
            return await handler.RegisterUrl(registerUri, requestModel);
        });
        app.MapPost("/pay", async (LipaNaMpesaRequestModel request, string orderId, [FromServices] ArpellaContext context) =>
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Orderid == orderId);
            if (order == null) return Results.NotFound("Order not found");
            if (order.Status == "Paid") return Results.BadRequest("Order already paid");
            var paymentResponse = await InitiateStkPush(request, orderId);
            return Results.Ok(new { message = "Payment Initiated!", response = paymentResponse });
        });
        app.MapPost("/mpesa/callback", (MpesaCallbackModel callback) => ReceiveMpesaCallback(callback));
    }

    #region Helpers
    private async Task<LipaNaMpesaResponseModel> InitiateStkPush(LipaNaMpesaRequestModel request, string orderId)
    {
        // **Fetch Order Details**
        var order = await _context.Orders.SingleOrDefaultAsync(o => o.Orderid == orderId);
        if (order == null) throw new Exception($"Order with ID {orderId} not found!");

        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"); ;
        string password = Convert.ToBase64String(Encoding.UTF8.GetBytes(_mpesaConfig.BusinessShortCode + _mpesaConfig.Passkey + timestamp));

        var requestPayload = new LipaNaMpesaRequestModel
        {
            BusinessShortCode = int.Parse(_mpesaConfig.BusinessShortCode),
            Password = password,
            Timestamp = timestamp,
            TransactionType = "CustomerBuyGoodsOnline",
            Amount = order.Total,
            PartyA = request.PhoneNumber,
            PartyB = _mpesaConfig.BusinessShortCode,
            PhoneNumber = request.PhoneNumber,
            CallBackUrl = _mpesaConfig.CallbackUri,
            AccountReference = "ArpellaStores",
            TransactionDescription = $"Payment for Order {orderId}"
        };

        string stkPushUri = "https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest";

        var response = await this._mpesaApiService.LipaNaMpesa(stkPushUri, requestPayload);

        if (response != null || !string.IsNullOrEmpty(response.MerchantRequestID))
        {
            var paymentRecord = new Payment
            {
                Orderid = orderId,
                Status = "Pending",
                TransactionId = response.CheckoutRequestID
            };
            _context.Payments.Add(paymentRecord);
            await _context.SaveChangesAsync();
        }
        return response;
    }
    private async Task<IResult> ReceiveMpesaCallback(MpesaCallbackModel callback)
    {
        var stk = callback.Body?.stkCallback;
        var metadata = stk?.CallbackMetadata?.Item;

        if (stk == null || metadata == null)
            return Results.BadRequest("Invalid callback structure.");

        var resultCode = stk.ResultCode;
        var resultDesc = stk.ResultDesc;
        var checkoutRequestId = stk.CheckoutRequestID;
        var receipt = _mpesaApiService.GetValue(metadata, "MpesaReceiptNumber");
        var phone = _mpesaApiService.GetValue(metadata, "PhoneNumber");
        var amount = _mpesaApiService.GetValue(metadata, "Amount");
        var transactionDesc = _mpesaApiService.GetValue(metadata, "TransactionDesc");

        string cacheKey = $"pending-order-{checkoutRequestId}";

        // 🟢 Successful payment (PIN entered, transaction completed)
        if (resultCode == 0)
        {
            if (_cache.TryGetValue<Order>(cacheKey, out var cachedOrder))
            {
                cachedOrder.Status = "Paid";
                _context.Orders.Add(cachedOrder);

                foreach (var item in cachedOrder.Orderitems)
                {
                    var orderItem = new Orderitem
                    {
                        OrderId = cachedOrder.Orderid,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };

                    var productWithInventory = await _context.Products
                        .Where(p => p.Id == orderItem.ProductId)
                        .Join(_context.Inventories,
                              product => product.InventoryId,
                              inventory => inventory.ProductId,
                              (product, inventory) => new
                              {
                                  Product = product,
                                  Inventory = inventory
                              })
                        .SingleOrDefaultAsync();

                    if (productWithInventory == null || productWithInventory.Inventory.StockQuantity < orderItem.Quantity)
                        return Results.BadRequest($"Product issue with '{orderItem.ProductId}'.");

                    productWithInventory.Inventory.StockQuantity -= orderItem.Quantity;
                    _context.Orderitems.Add(orderItem);
                    _context.Inventories.Update(productWithInventory.Inventory);
                }

                var payment = new Payment
                {
                    Orderid = cachedOrder.Orderid,
                    Status = "Completed",
                    TransactionId = receipt,
                };
                _context.Payments.Add(payment);

                await _context.SaveChangesAsync();
                _cache.Remove(cacheKey);
                return Results.Ok("Payment received. Order saved successfully.");
            }
            else
            {
                return Results.BadRequest("No pending order found for this transaction.");
            }
        }
        else
        {
             // Payment failed or cancelled
            _cache.Remove(cacheKey);
            return Results.BadRequest($"Payment failed: {resultDesc}");
        }
    }
    #endregion
}

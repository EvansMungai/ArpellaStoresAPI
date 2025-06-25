using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Extensions;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace ArpellaStores.Features.PaymentManagement.Endpoints;

public class PaymentRoutes : IRouteRegistrar
{
    private readonly IMpesaApiService _mpesaApiService;
    private readonly MpesaConfig _mpesaConfig;
    private readonly ArpellaContext _context;

    public PaymentRoutes(IMpesaApiService mpesaApiService, IOptions<MpesaConfig> mpesaConfig, ArpellaContext context)
    {
        _mpesaApiService = mpesaApiService;
        _mpesaConfig = mpesaConfig.Value;
        _context = context;
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
        app.MapPost("/pay", async (LipaNaMpesaRequestModel request, string orderId) =>
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Orderid == orderId);
            if (order == null) return Results.NotFound("Order not found");
            if (order.Status == "Paid") return Results.BadRequest("Order already paid");
            var paymentResponse = await InitiateStkPush(request, orderId);
            return Results.Ok(new { message = "Payment Initiated!", response = paymentResponse });
        });
        app.MapPost("/mpesa/callback", (LipaNaMpesaResponseModel callback) => ReceiveMpesaCallback(callback));
    }

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
            PartyB = int.Parse(_mpesaConfig.BusinessShortCode),
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
    private async Task<IResult> ReceiveMpesaCallback(LipaNaMpesaResponseModel callbackData)
    {
        Console.WriteLine($"Received M-Pesa Callback: {JsonConvert.SerializeObject(callbackData)}");

        // Validate Callback data
        if (callbackData == null || string.IsNullOrEmpty(callbackData.CheckoutRequestID)) return Results.BadRequest("Invalid callback data");

        // Find payment record using CheckoutRequestId
        var paymentRecord = await _context.Payments.SingleOrDefaultAsync(p => p.TransactionId ==  callbackData.CheckoutRequestID);
        if (paymentRecord == null) Results.NotFound($"Payment record for Transaction ID {callbackData.CheckoutRequestID} not found");

        var order = await _context.Orders.SingleOrDefaultAsync(o => o.Orderid == paymentRecord.Orderid);
        if (order == null) Results.NotFound($"Order with ID {paymentRecord.Orderid} not found");

        // Check if payment was successfull
        if(callbackData.ResponseCode == 0)
        {
            order.Status = "Paid";
            paymentRecord.Status = "Paid";
            paymentRecord.TransactionId = callbackData.MpesaReceiptNumber;
            await _context.SaveChangesAsync();

            return Results.Ok(new { message = "Payment successful", orderId = order.Orderid });
        }
        return Results.BadRequest(new { message = "Payment failed", reason = callbackData.ResponseDescription });
    }
}

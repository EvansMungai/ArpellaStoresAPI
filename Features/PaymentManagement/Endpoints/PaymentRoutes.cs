using ArpellaStores.Extensions;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;
using Microsoft.Extensions.Options;
using System.Text;

namespace ArpellaStores.Features.PaymentManagement.Endpoints;

public class PaymentRoutes : IRouteRegistrar
{
    private readonly IMpesaApiService _mpesaApiService;
    private readonly MpesaConfig _mpesaConfig;

    public PaymentRoutes(IMpesaApiService mpesaApiService, IOptions<MpesaConfig> mpesaConfig)
    {
        _mpesaApiService = mpesaApiService;
        _mpesaConfig = mpesaConfig.Value;
    }

    public void RegisterRoutes(WebApplication app)
    {
        MapPaymentRoutes(app);
    }

    public void MapPaymentRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Mpesa");
        app.MapGet("/access-token", () => this._mpesaApiService.GenerateAccessToken());
        app.MapGet("register-url", async () =>
        {
            string registerUri = "https://api.safaricom.co.ke/mpesa/c2b/v1/registerurl";
            var requestModel = new RegisterUrlRequestModel
            {
                ShortCode = 5142142,
                ResponseType = "Completed",
                ConfirmationUrl = _mpesaConfig.ConfirmationUri,
                ValidationUrl = _mpesaConfig.ValidationUri
            };
            return await this._mpesaApiService.RegisterUrl(registerUri, requestModel);
        });
        app.MapPost("/pay", (LipaNaMpesaRequestModel request) => InitiateStkPush(request));
    }

    private async Task<LipaNaMpesaResponseModel> InitiateStkPush(LipaNaMpesaRequestModel request)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string password = Convert.ToBase64String(Encoding.UTF8.GetBytes(_mpesaConfig.BusinessShortCode + _mpesaConfig.Passkey + timestamp));

        var requestPayload = new LipaNaMpesaRequestModel
        {
            BusinessShortCode = int.Parse(_mpesaConfig.BusinessShortCode),
            Password = password,
            Timestamp = timestamp,
            Amount = request.Amount,
            PartyA = request.PhoneNumber,
            PartyB = int.Parse(_mpesaConfig.BusinessShortCode),
            PhoneNumber = request.PhoneNumber,
            CallBackUrl = _mpesaConfig.CallbackUri,
            AccountReference = "ArpellaStores",
            TransactionDescription = "Tuma Kitu"
        };

        string stkPushUri = "https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest";

        return await this._mpesaApiService.LipaNaMpesa(stkPushUri, requestPayload);
    }
}

using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderPaymentService : IOrderPaymentService
{
    private readonly IMpesaApiService _mpesaApiService;
    private readonly MpesaConfig _mpesaConfig;
    public OrderPaymentService(IMpesaApiService api, IOptions<MpesaConfig> config)
    {
        _mpesaApiService = api;
        _mpesaConfig = config.Value;
    }

    public async Task<LipaNaMpesaResponseModel> InitiateStkPushAsync(CachedOrderDto order)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string password = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(_mpesaConfig.BusinessShortCode + _mpesaConfig.Passkey + timestamp));
        var callbackUri = Environment.GetEnvironmentVariable("MpesaConfig__CallbackUri");

        var payload = new LipaNaMpesaRequestModel
        {
            BusinessShortCode = int.Parse(_mpesaConfig.BusinessShortCode),
            Password = password,
            Timestamp = timestamp,
            TransactionType = "CustomerBuyGoodsOnline",
            Amount = order.Total,
            PartyA = order.PhoneNumber,
            PartyB = _mpesaConfig.TillNumber,
            PhoneNumber = order.PhoneNumber,
            CallBackUrl = callbackUri,
            AccountReference = "ArpellaStores",
            TransactionDescription = order.Orderid
        };

        string uri = "https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest";
        return await _mpesaApiService.LipaNaMpesa(uri, payload);
    }
}

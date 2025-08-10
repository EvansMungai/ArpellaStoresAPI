using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;
using Microsoft.Extensions.Options;
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

    public async Task<LipaNaMpesaResponseModel> InitiateStkPushAsync(Order order)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string password = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(_mpesaConfig.BusinessShortCode + _mpesaConfig.Passkey + timestamp));

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
            CallBackUrl = _mpesaConfig.CallbackUri,
            AccountReference = "ArpellaStores",
            TransactionDescription = order.Orderid
        };

        string uri = "https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest";
        return await _mpesaApiService.LipaNaMpesa(uri, payload);
    }
}

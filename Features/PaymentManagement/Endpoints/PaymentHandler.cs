using ArpellaStores.Extensions;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;

namespace ArpellaStores.Features.PaymentManagement.Endpoints;

public class PaymentHandler : IHandler
{
    public static string RouteName => "Payment Management";
    private readonly IMpesaApiService _mpesaApiService;
    public PaymentHandler(IMpesaApiService mpesaApiService)
    {
        _mpesaApiService = mpesaApiService;
    }

    public async Task<OAuthResponseModel> GenerateAccessToken() => await _mpesaApiService.GenerateAccessToken();
    public async Task<CommonResponseModel> RegisterUrl(string c2BRegisterUrl, RegisterUrlRequestModel registerUriModel) => await _mpesaApiService.RegisterUrl(c2BRegisterUrl, registerUriModel);
    public async Task<LipaNaMpesaResponseModel> LipaNaMpesa(string lipaNaMpesaUri, LipaNaMpesaRequestModel lipaNaMpesaModel) => await _mpesaApiService.LipaNaMpesa(lipaNaMpesaUri, lipaNaMpesaModel);
}

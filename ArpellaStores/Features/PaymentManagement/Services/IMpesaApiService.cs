using ArpellaStores.Features.PaymentManagement.Models;

namespace ArpellaStores.Features.PaymentManagement.Services;

public interface IMpesaApiService
{
    Task<OAuthResponseModel> GenerateAccessToken();
    Task<CommonResponseModel> RegisterUrl(string c2BRegisterUrl, RegisterUrlRequestModel registerUriModel);
    //Task<CommonResponseModel> C2B(string oAuthUri, string c2BSimulateTransactionUri, C2BRequestModel c2BModel, string encodedSecret);
    //Task<B2CResponseModel> B2C(string oAuthUri, string b2CUri, B2CRequestModel b2CModel, string encodedSecret);
    Task<LipaNaMpesaResponseModel> LipaNaMpesa(string lipaNaMpesaUri, LipaNaMpesaRequestModel lipaNaMpesaModel);
    string GetValue(List<CallbackItem> items, string key);
    //Task<LipaNaMpesaQueryResponseModel> LipaNaMpesaQuery(string oAuthUri, string lipaNaMpesaQueryUri, LipaNaMpesaQueryRequestModel lipaNaMpesaQueryViewModel, string encodedSecret);
    //Task<DynamicQrResponseModel> DynamicQr(string oAuthUri, string dynamicQrUri, DynamicQrRequestModel dynamicQrRequestModel, string encodedSecret);
    //Task<CommonResponseModel> Reversal(string oAuthUri, string reversalUri, ReversalRequestModel reversalRequestModel, string encodedSecret);
    //Task<CommonResponseModel> AccountBalance(string oAuthUri, string accountBalanceUri, AccountBalanceRequestModel accountBalanceRequestModel, string encodedSecret);

}

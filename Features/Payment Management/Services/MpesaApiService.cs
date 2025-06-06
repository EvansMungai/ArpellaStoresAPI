using ArpellaStores.Features.Payment_Management.Common;
using ArpellaStores.Features.Payment_Management.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace ArpellaStores.Features.Payment_Management.Services;

/// <summary>
/// The MPESA Application Programming Interface
/// </summary>
public class MpesaApiService : IMpesaApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly MpesaConfig _mpesaConfig;

    public MpesaApiService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache, IOptions<MpesaConfig> mpesaConfig)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _cache = cache;
        _mpesaConfig = mpesaConfig.Value;
    }

    #region Utilities

    public async Task<OAuthResponseModel> GenerateAccessToken()
    {
        if (_cache.TryGetValue("MpesaAccessToken", out OAuthResponseModel cachedToken)) return cachedToken;

        string encodedKeySecret = GenerateEncodedKeySecret();
        string oAuthUri = "https://api.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials";
        //Remove initial authorization header
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        //set Authorization header 
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedKeySecret);
        //send request            
        var response = await _httpClient.GetAsync(oAuthUri);
        //ensure success
        response.EnsureSuccessStatusCode();
        //stringify content
        var resultContent = await response.Content.ReadAsStringAsync();
        //deserialize object
        var result = JsonConvert.DeserializeObject<OAuthResponseModel>(resultContent);
        // Store the token in cache with expiration
        _cache.Set("MpesaAccessToken", result, TimeSpan.FromSeconds(double.Parse(result.ExpiresIn) - 300));

        return result;
    }

    private async Task<T> GetMpesaResponseAsync<T, U>(string requestUri, U requestModel)
    {
        //Get the access token
        var accessTokenModel = await GenerateAccessToken();
        //serialize request model
        var serializedObject = JsonConvert.SerializeObject(requestModel);
        //set the content
        var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
        //Remove initial authorization header
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        Console.WriteLine($"This is the access token: {accessTokenModel.AccessToken}");
        //Set current authorization header
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessTokenModel.AccessToken);
        //send request
        var response = await _httpClient.PostAsync(requestUri, content);
        //ensure success
        response.EnsureSuccessStatusCode();
        //stringify content
        var resultContent = await response.Content.ReadAsStringAsync();
        //deserialize object
        var result = JsonConvert.DeserializeObject<T>(resultContent);

        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// This API enables you to register the callback URLs via which you shall receive payment notifications for payments to your paybill/till number.
    /// </summary>
    /// <param name="c2BRegisterUrl">C2B register url model</param>
    /// <param name="registerUriModel">RegisterUrl model</param>
    /// <param name="oAuthModel">OAth model</param>
    /// <returns>RegisterUrlResponse model</returns>
    public async Task<CommonResponseModel> RegisterUrl(string c2BRegisterUrl, RegisterUrlRequestModel registerUriModel)
    {
        var result = await GetMpesaResponseAsync<CommonResponseModel, RegisterUrlRequestModel>(c2BRegisterUrl, registerUriModel);

        return result;
    }

    /// <summary>
    /// This API is used to simulate payment requests from clients and to your API. 
    /// It basically simulates a payment made from the client phone's STK/SIM Toolkit menu, and enables you to receive the payment requests in real time.
    /// </summary>
    /// <param name="c2BSimulateTransactionUri">C2B simulate transaction uri</param>
    /// <param name="c2BModel">C2B model</param>
    /// <param name="oAuthModel">OAuth model</param>
    /// <returns>C2BResponse model</returns>
    //public async Task<CommonResponseModel> C2B(string oAuthUri, string c2BSimulateTransactionUri, C2BRequestModel c2BModel, string encodedSecret)
    //{
    //    var result = await GetMpesaResponseAsync<CommonResponseModel, C2BRequestModel>(oAuthUri, c2BSimulateTransactionUri, c2BModel, encodedSecret);

    //    return result;
    //}

    /// <summary>
    /// Also known as the Bulk Payment API, the Business to Consumer (B2C) API enables a Business or Organization to pay their customers for a variety of reasons.
    /// </summary>
    /// <param name="b2CUri">B2C uri</param>
    /// <param name="b2CModel">B2C model</param>
    /// <param name="oAuthModel">OAuth model</param>
    /// <returns>B2CResponse model</returns>
    //public async Task<B2CResponseModel> B2C(string oAuthUri, string b2CUri, B2CRequestModel b2CModel, string encodedSecret)
    //{
    //    var result = await GetMpesaResponseAsync<B2CResponseModel, B2CRequestModel>(oAuthUri, b2CUri, b2CModel, encodedSecret);

    //    return result;
    //}

    /// <summary>
    /// The Lipa na M-Pesa (LNM) API is a C2B API designed to utilize the new feature introduced by Safaricom known as STK Push.
    /// </summary>
    /// <param name="lipaNaMpesaUri">Lipa na mpesa uri</param>
    /// <param name="lipaNaMpesaModel">LipaNaMpesa model</param>
    /// <returns>LipaNaMpesaResponse model</returns>
    public async Task<LipaNaMpesaResponseModel> LipaNaMpesa(string lipaNaMpesaUri, LipaNaMpesaRequestModel lipaNaMpesaModel)
    {
        var result = await GetMpesaResponseAsync<LipaNaMpesaResponseModel, LipaNaMpesaRequestModel>(lipaNaMpesaUri, lipaNaMpesaModel);

        return result;
    }

    /// <summary>
    /// Use this API to check the status of a Lipa Na M-Pesa Online Payment.
    /// </summary>
    /// <param name="lipaNaMpesaQueryUri">Lipa na mpesa query uri</param>
    /// <param name="lipaNaMpesaQueryModel">LipaNaMpesaQuery model</param>
    /// <param name="oAuthModel">OAuth model</param>
    /// <returns>LipaNaMpesaQueryResponse model</returns>
    //public async Task<LipaNaMpesaQueryResponseModel> LipaNaMpesaQuery(string oAuthUri, string lipaNaMpesaQueryUri, LipaNaMpesaQueryRequestModel lipaNaMpesaQueryViewModel, string encodedSecret)
    //{
    //    var result = await GetMpesaResponseAsync<LipaNaMpesaQueryResponseModel, LipaNaMpesaQueryRequestModel>(oAuthUri, lipaNaMpesaQueryUri, lipaNaMpesaQueryViewModel, encodedSecret);

    //    return result;
    //}

    /// <summary>
    /// Use this api to generate a dynamic qr code
    /// </summary>
    /// <param name="oAuthUri">OAuth Uri</param>
    /// <param name="dynamicQrUri">Dynamic qr uri</param>
    /// <param name="dynamicQrRequestModel">Dynamic qr request model</param>
    /// <param name="encodedSecret">Encoded secret</param>
    /// <returns>Dynamic qr response</returns>
    //public async Task<DynamicQrResponseModel> DynamicQr(string oAuthUri, string dynamicQrUri, DynamicQrRequestModel dynamicQrRequestModel, string encodedSecret)
    //{
    //    var result = await GetMpesaResponseAsync<DynamicQrResponseModel, DynamicQrRequestModel>(oAuthUri, dynamicQrUri, dynamicQrRequestModel, encodedSecret);

    //    return result;
    //}

    /// <summary>
    /// Use this API to initial an m-pesa reversal request
    /// </summary>
    /// <param name="oAuthUri">oAuth Uri</param>
    /// <param name="reversalUri">Reversal Uri</param>
    /// <param name="reversalRequestModel">Reversal request model</param>
    /// <param name="encodedSecret">encoded secret</param>
    /// <returns>Common response model</returns>
    //public async Task<CommonResponseModel> Reversal(string oAuthUri, string reversalUri, ReversalRequestModel reversalRequestModel, string encodedSecret)
    //{
    //    var result = await GetMpesaResponseAsync<CommonResponseModel, ReversalRequestModel>(oAuthUri, reversalUri, reversalRequestModel, encodedSecret);

    //    return result;
    //}

    /// <summary>
    /// Represents an account balance
    /// </summary>
    /// <param name="oAuthUri">oAuth Uri</param>
    /// <param name="accountBalanceUri">Account balance uri</param>
    /// <param name="accountBalanceRequestModel">Account balance request model</param>
    /// <param name="encodedSecret">encoded secret</param>
    /// <returns>Common response model</returns>
    //public async Task<CommonResponseModel> AccountBalance(string accountBalanceUri, AccountBalanceRequestModel accountBalanceRequestModel)
    //{
    //    var result = await GetMpesaResponseAsync<CommonResponseModel, AccountBalanceRequestModel>(accountBalanceUri, accountBalanceRequestModel);

    //    return result;
    //}

    #endregion

    #region Helpers 
    public string GenerateEncodedKeySecret()
    {
        string consumerKey = _mpesaConfig.ConsumerKey;
        string consumerSecret = _mpesaConfig.ConsumerSecret;

        if (string.IsNullOrEmpty(consumerKey) || string.IsNullOrEmpty(consumerSecret))
        {
            throw new Exception("ConsumerKey or ConsumerSecret is missing in appsettings.json.");
        }

        string keySecret = $"{consumerKey}:{consumerSecret}";
        byte[] keySecretBytes = Encoding.UTF8.GetBytes(keySecret);
        return Convert.ToBase64String(keySecretBytes);
    }
    #endregion
}

using ArpellaStores.Features.Payment_Management.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ArpellaStores.Services;

public class MpesaService : IMpesaService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _cache;
    private readonly MpesaSettings _mpesa;
    private const string TokenCachedKey = "MpesaAccessToken";
    public MpesaService(IHttpClientFactory clientFactory, IMemoryCache memoryCache, IOptions<MpesaSettings> mpesaConfig)
    {
        _clientFactory = clientFactory;
        _cache = memoryCache;
        _mpesa = mpesaConfig.Value;
    }

    public async Task<string> GetToken()
    {
        string url = "https://api.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials";
        if (_cache.TryGetValue(TokenCachedKey, out string cachedToken)) return cachedToken;

        using (var client = _clientFactory.CreateClient()) {
            var byteArray = Encoding.ASCII.GetBytes($"{_mpesa.ConsumerKey}:{_mpesa.ConsumerSecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            string accessToken = JsonConvert.DeserializeObject<MpesaTokenResponse>(responseString)?.access_token;

            if (accessToken != null) _cache.Set(TokenCachedKey, accessToken, TimeSpan.FromMinutes(58));

            return accessToken;
        }
    }
    public async Task<string> RegisterUrls()
    {
        var jsonBody = JsonConvert.SerializeObject(new
        {
            ValidationURL = "https://mydomain.com/confirmation",
            ConfirmationURL = "https://mydomain.com/validation",
            ResponseType = "Completed",
            ShortCode = 600991
        });

        var jsonReadyBody = new StringContent(jsonBody.ToString(), Encoding.UTF8, "application/json");

        var token = await GetToken();

        var client = _clientFactory.CreateClient("mpesa");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var url = "/mpesa/c2b/v1/registerurl";

        var response = await client.PostAsync(url, jsonReadyBody);

        return await response.Content.ReadAsStringAsync();
    }
    public async Task<string> SendPaymentPrompt(MpesaExpressRequest mpesa)
    {
        string accessToken = await GetToken();
        string url = "https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest";
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var password = GeneratePassword(_mpesa.ShortCode, timestamp, _mpesa.Passkey);

        var requestBody = new MpesaExpressRequest
        {
            BusinessShortCode = _mpesa.ShortCode,
            Password = password,
            Timestamp = timestamp,
            TransactionType = "CustomerBuyGoodsOnline",
            Amount = 1,
            PartyA = mpesa.PhoneNumber,
            PartyB = _mpesa.ShortCode,
            PhoneNumber = mpesa.PhoneNumber,
            CallbackUrl = "https://164.68.99.188:8080/api/mpesa",
            AccountReference = "Arpella Store",
            TransactionDesc = "Test Payment"
        };

        using (var client = _clientFactory.CreateClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

    }
    #region Utilities
    public string GeneratePassword(string businessShortCode, string passkey, string timestamp)
    {
        string dataToEncode = businessShortCode + passkey + timestamp;
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataToEncode);
        string password = Convert.ToBase64String(dataBytes);
        return password;
    }
    #endregion
}

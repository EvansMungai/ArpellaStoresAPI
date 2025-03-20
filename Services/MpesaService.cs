using ArpellaStores.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ArpellaStores.Services;

public class MpesaService : IMpesaService
{
    private readonly IHttpClientFactory _clientFactory;
    public MpesaService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> GetToken()
    {
        var client = _clientFactory.CreateClient("mpesa");
        var authString = "k8EZfIlFxPDAA8nXfQFZRiTKeSCMWExcIpWC1QnTIYnelQGu:GdNyI5JHC2jpiV7481K9B3VPkuDy2NeSPtpI6dGmUZJsGOyGE61kAJ63JJ8hPlx3";
        var encodedString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authString));
        var _url = "/oauth/v1/generate?grant_type=client_credentials";
        var request = new HttpRequestMessage(HttpMethod.Get, _url);
        request.Headers.Add("Authorization", $"Basic {encodedString}");
        var response = await client.SendAsync(request);
        var mpesaResponse = await response.Content.ReadAsStringAsync();
        Token tokenObject = JsonConvert.DeserializeObject<Token>(mpesaResponse);

        return tokenObject.access_token;
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
    public async Task<string> SendPaymentPrompt(MpesaExpress mpesa)
    {
        mpesa.Timestamp= DateTime.Now.ToString("yyyyMMddHHmmss");
        var client = _clientFactory.CreateClient("mpesa");
        var _url = "/mpesa/stkpush/v1/processrequest";
        var token = await GetToken();
        var password= GeneratePassword(mpesa.BusinessShortCode, mpesa.Password, mpesa.Timestamp);

        var requestBody = new
        {
            BusinessShortCode = mpesa.BusinessShortCode,
            Password = password,
            Timestamp = mpesa.Timestamp,
            TransactionType = mpesa.TransactionType,
            Amount = mpesa.Amount,
            PartyA = mpesa.PartyA,
            PartyB = mpesa.PartyB,
            PhoneNumber = mpesa.PhoneNumber,
            AccountReference = mpesa.AccountReference,
            CallbackUrl = mpesa.CallbackUrl,
            TransactionDescription = mpesa.TransactionDesc,
        };
        var jsonString = JsonConvert.SerializeObject(requestBody);
        var jsonReadyBody = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, _url)
        {
            Content = jsonReadyBody
        };
        request.Headers.Add("Authorization", $"Bearer {token}");
        HttpResponseMessage response = await client.SendAsync(request);
        string responseContent = await response.Content.ReadAsStringAsync();
        return responseContent;
    }
    #region Utilities
    public string GeneratePassword(int businessShortCode, string passkey, string timestamp)
    {
        string dataToEncode = businessShortCode + passkey + timestamp;
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataToEncode);
        string password = Convert.ToBase64String(dataBytes);
        return password;
    }
    #endregion
}

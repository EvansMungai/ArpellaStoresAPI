using ArpellaStores.Models;
using Newtonsoft.Json;

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
}

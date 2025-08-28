
using ArpellaStores.Features.SmsManagement.Models;
using Microsoft.Extensions.Options;

namespace ArpellaStores.Features.SmsManagement.Services;

public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly HostPinnacleOptions _options;
    private readonly ILogger<SmsService> _logger;
    public SmsService(HttpClient httpClient, IOptions<HostPinnacleOptions> options, ILogger<SmsService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }
    public async Task<string> SendQuickSMSAsync(string message, string mobile)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message must not be empty.", nameof(message));
        if (string.IsNullOrWhiteSpace(mobile))
            throw new ArgumentException("Mobile must not be empty", nameof(mobile));

        var payload = new Dictionary<string, string>
        {
            {"userid", _options.Username },
            {"password", _options.Password},
            {"msg", message },
            {"sendMethod", "quick"},
            {"senderid", _options.SenderId},
            {"msgType", "text"},
            {"mobile", mobile },
            {"duplicatecheck", "true"},
            {"output", "json"}
        };
        string uri = "https://smsportal.hostpinnacle.co.ke/SMSApi/send";
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new FormUrlEncodedContent(payload)
        };
        request.Headers.Add("apikey", _options.ApiKey);
        request.Headers.Add("cache-control", "no-cache");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _logger.LogInformation($"This is the hostpinnacle sms api response: {responseBody}.");
        return responseBody;
    }
}

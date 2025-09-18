
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
    public async Task<string> SendBatchSMSAsync(string message, List<string> phoneNumbers)
    {
        _logger.LogInformation("Starting Send Batch Sms Function.");
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message must not be empty.", nameof(message));
        if (phoneNumbers == null || !phoneNumbers.Any())
            throw new ArgumentException("Phone number list must not be empty", nameof(phoneNumbers));
        Console.WriteLine(phoneNumbers.Select(p => p.Trim().Replace("+", "").Replace(" ", "").Replace("-", "")));
        _logger.LogInformation($"This are the phone numbers to be sent the order creation message. {phoneNumbers.Select(p => p.Trim().Replace("+", "").Replace(" ", "").Replace("-", ""))}");

        var payload = new Dictionary<string, string>
        {
            {"userid", _options.Username },
            {"password", _options.Password},
            {"msg", message },
            {"sendMethod", "quick"},
            {"senderid", _options.SenderId},
            {"msgType", "text"},
            {"mobile", string.Join(",", phoneNumbers.Select(p => p.Trim().Replace("+", "").Replace(" ", "").Replace("-", "")))},
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

        _logger.LogInformation("Sending request to hostpinnacle.");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        _logger.LogInformation("Hostpinnacle has responded with a success status code.");
        var responseBody = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Returning Response body.");
        return responseBody;
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
        return responseBody;
    }
}

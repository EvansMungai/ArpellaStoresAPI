namespace ArpellaStores.Features.SmsManagement.Services;

public interface ISmsService
{
    Task<string> SendBatchSMSAsync(string message, List<string> phoneNumbers);
    Task<string> SendQuickSMSAsync(string message, string mobile);
}

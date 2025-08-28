namespace ArpellaStores.Features.SmsManagement.Services;

public interface ISmsService
{
    Task<string> SendQuickSMSAsync(string message, string mobile);
}

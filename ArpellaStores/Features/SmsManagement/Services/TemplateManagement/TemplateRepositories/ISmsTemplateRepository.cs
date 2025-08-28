using ArpellaStores.Features.SmsManagement.Models;

namespace ArpellaStores.Features.SmsManagement.Services;

public interface ISmsTemplateRepository
{
    Task<List<SmsTemplate>> GetSmsTemplatesAsync();
    Task<SmsTemplate> GetSmsTemplateAsync(string templateType);
    Task AddTemplateAsync(SmsTemplate smsTemplate);
    Task RemoveTemplateAsync(string templateType);

}

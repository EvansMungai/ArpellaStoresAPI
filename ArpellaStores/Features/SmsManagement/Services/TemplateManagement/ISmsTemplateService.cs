using ArpellaStores.Features.SmsManagement.Models;

namespace ArpellaStores.Features.SmsManagement.Services;

public interface ISmsTemplateService
{
    Task<IResult> GetAllTemplatesAsync();
    Task<IResult> GetTemplateByTypeAsync(string templateType);
    Task<IResult> CreateTemplateAsync(SmsTemplate smsTemplate);
    Task<IResult> RemoveTemplateAsync(string templateType);
}

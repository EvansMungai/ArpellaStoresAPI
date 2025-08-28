using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.SmsManagement.Models;
using ArpellaStores.Features.SmsManagement.Services;

namespace ArpellaStores.Features.SmsManagement.Endpoints;

public class SmsTemplateHandler : IHandler
{
    public static string RouteName => "Sms Template Management";
    private readonly ISmsTemplateService _smsTemplateService;
    public SmsTemplateHandler(ISmsTemplateService smsTemplateService)
    {
        _smsTemplateService = smsTemplateService;
    }
    public Task<IResult> GetAllSmsTemplatesAsync() => _smsTemplateService.GetAllTemplatesAsync();
    public Task<IResult> GetSmsTemplateByTypeAsync(string templateType) => _smsTemplateService.GetTemplateByTypeAsync(templateType);
    public Task<IResult> CreateSmsTemplateAsync(SmsTemplate smsTemplate) => _smsTemplateService.CreateTemplateAsync(smsTemplate);
    public Task<IResult> RemoveSmsTemplateAsync(string templateType) => _smsTemplateService.RemoveTemplateAsync(templateType);

}

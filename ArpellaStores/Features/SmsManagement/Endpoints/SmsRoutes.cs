using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.SmsManagement.Models;

namespace ArpellaStores.Features.SmsManagement.Endpoints;

public class SmsRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapSmsTemplateRoutes(app);
    }
    public void MapSmsTemplateRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Sms Templates");
        app.MapGet("/sms-templates", (SmsTemplateHandler handler) => handler.GetAllSmsTemplatesAsync());
        app.MapGet("/sms-template/{templateType}", (SmsTemplateHandler handler, string templateType) => handler.GetSmsTemplateByTypeAsync(templateType));
        app.MapPost("/sms-template", (SmsTemplateHandler handler, SmsTemplate template) => handler.CreateSmsTemplateAsync(template));
        app.MapDelete("/sms-template/{templateType}", (SmsTemplateHandler handler, string templateType) => handler.RemoveSmsTemplateAsync(templateType));
    }
}

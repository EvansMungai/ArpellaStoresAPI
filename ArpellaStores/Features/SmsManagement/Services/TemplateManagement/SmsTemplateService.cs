using ArpellaStores.Features.SmsManagement.Models;

namespace ArpellaStores.Features.SmsManagement.Services;

public class SmsTemplateService : ISmsTemplateService
{
    private readonly ISmsTemplateRepository _repo;
    public SmsTemplateService(ISmsTemplateRepository repo)
    {
        _repo = repo;
    }

    public async Task<IResult> CreateTemplateAsync(SmsTemplate smsTemplate)
    {
        try
        {
            await _repo.AddTemplateAsync(smsTemplate);
            return Results.Ok(smsTemplate);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }

    public async Task<IResult> GetAllTemplatesAsync()
    {
        var templates = await _repo.GetSmsTemplatesAsync();
        return templates == null || templates.Count == 0 ? Results.NotFound("No Sms Templates were found.") : Results.Ok(templates);
    }

    public async Task<IResult> GetTemplateByTypeAsync(string templateType)
    {
        var template = await _repo.GetSmsTemplateAsync(templateType);
        return template == null ? Results.NotFound($"Sms Template of type {templateType} was not found.") : Results.Ok(template);
    }

    public async Task<IResult> RemoveTemplateAsync(string templateType)
    {
        var retrievedTemplate = await _repo.GetSmsTemplateAsync(templateType);
        if (retrievedTemplate == null)
            return Results.NotFound($"Sms Template of type {templateType} was not found.");

        try
        {
            await _repo.RemoveTemplateAsync(templateType);
            return Results.Ok(retrievedTemplate);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
}

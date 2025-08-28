using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.SmsManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.SmsManagement.Services;

public class SmsTemplateRepository : ISmsTemplateRepository
{
    private readonly ArpellaContext _context;
    public SmsTemplateRepository(ArpellaContext context)
    {
        _context = context;
    }
    public async Task AddTemplateAsync(SmsTemplate smsTemplate)
    {
        _context.SmsTemplates.Add(smsTemplate);
        await _context.SaveChangesAsync();
    }

    public async Task<SmsTemplate> GetSmsTemplateAsync(string templateType)
    {
        return await _context.SmsTemplates.Select(t => new SmsTemplate { Id = t.Id, TemplateType = t.TemplateType, Content = t.Content, CreatedAt = t.CreatedAt, UpdatedAt = t.UpdatedAt }).AsNoTracking().SingleOrDefaultAsync(t => t.TemplateType == templateType);
    }

    public async Task<List<SmsTemplate>> GetSmsTemplatesAsync()
    {
        return await _context.SmsTemplates.Select(t => new SmsTemplate{Id = t.Id, TemplateType = t.TemplateType, Content = t.Content, CreatedAt = t.CreatedAt, UpdatedAt = t.UpdatedAt}).AsNoTracking().ToListAsync();
    }

    public async Task RemoveTemplateAsync(string templateType)
    {
        SmsTemplate? retrievedTemplate = await _context.SmsTemplates.AsNoTracking().SingleOrDefaultAsync(t => t.TemplateType == templateType);
        if (retrievedTemplate == null) return;

        _context.SmsTemplates.Remove(retrievedTemplate);
        await _context.SaveChangesAsync();
    }
}

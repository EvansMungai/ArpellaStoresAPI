using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.SettingManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.SettingManagement.Services;

public class SettingService : ISettingService
{
    private readonly ArpellaContext _context;
    public SettingService(ArpellaContext context)
    {
        _context = context;
    }

    public async Task<IResult> CreateSettingObject(Setting setting)
    {
        var local = _context.Settings.Local.FirstOrDefault(s => s.Id == setting.Id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existing = await _context.Settings.AsNoTracking().SingleOrDefaultAsync(s => s.Id == setting.Id);
        if (existing != null)
            return Results.Conflict($"A settings record with settingID = {setting.Id} already exists.");

        var settingRecord = new Setting
        {
            SettingName = setting.SettingName,
            SettingValue = setting.SettingValue
        };
        try
        {
            _context.Settings.Add(settingRecord);
            await _context.SaveChangesAsync();
            return Results.Ok(settingRecord);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<IResult> GetAllSettings()
    {
        var settings = await _context.Settings.Select(s => new { s.Id, s.SettingName, s.SettingValue }).AsNoTracking().ToListAsync();
        return settings == null || settings.Count == 0 ? Results.NotFound("No settings record found") : Results.Ok(settings);
    }

    public async Task<IResult> GetSetting(int settingId)
    {
        var setting = await _context.Settings.Select(s => new { s.Id, s.SettingName, s.SettingValue }).AsNoTracking().SingleOrDefaultAsync(s => s.Id == settingId);
        return setting == null ? Results.NotFound($"Settings record with setting id = {settingId} was not found") : Results.Ok(setting);
    }

    public async Task<IResult> RemoveSettingObject(int settingId)
    {
        var local = _context.Settings.Local.FirstOrDefault(s => s.Id == settingId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var retrievedSettingRecord= await _context.Settings.SingleOrDefaultAsync(s => s.Id == settingId);
        if (retrievedSettingRecord != null)
        {
            _context.Settings.Remove(retrievedSettingRecord);
            await _context.SaveChangesAsync();
            return Results.Ok(retrievedSettingRecord);
        }
        else
        {
            return Results.NotFound($"Setting with settingId =  {settingId} was not found");
        }
    }

    public async Task<IResult> UpdateSettingDetails(Setting update, int settingId)
    {
        var local = _context.Settings.Local.FirstOrDefault(s => s.Id == settingId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var retrievedSettingRecord = await _context.Settings.SingleOrDefaultAsync(s => s.Id == settingId);
        if (retrievedSettingRecord != null)
        {
            retrievedSettingRecord.SettingName = update.SettingName;
            retrievedSettingRecord.SettingValue = update.SettingValue;

            try
            {
                _context.Settings.Update(retrievedSettingRecord);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedSettingRecord);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Setting with settingId = {settingId} was not found");
        }
    }
}

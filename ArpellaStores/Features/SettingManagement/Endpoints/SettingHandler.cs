using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.SettingManagement.Models;
using ArpellaStores.Features.SettingManagement.Services;

namespace ArpellaStores.Features.SettingManagement.Endpoints;

public class SettingHandler : IHandler
{
    public static string RouteName => "Settings Management";
    private readonly ISettingService _settingService;
    public SettingHandler(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public Task<IResult> GetSettings() => _settingService.GetAllSettings();
    public Task<IResult> GetSetting(int settingId) => _settingService.GetSetting(settingId);
    public Task<IResult> CreateSetting(Setting setting) => _settingService.CreateSettingObject(setting);
    public Task<IResult> UpdateSettingDetails(Setting update, int settingId) => _settingService.UpdateSettingDetails(update, settingId);
    public Task<IResult> RemoveSettingObject(int settingId) => _settingService.RemoveSettingObject(settingId);
}

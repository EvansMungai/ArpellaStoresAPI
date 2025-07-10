using ArpellaStores.Features.SettingManagement.Models;

namespace ArpellaStores.Features.SettingManagement.Services;

public interface ISettingService
{
    Task<IResult> GetAllSettings();
    Task<IResult> GetSetting(int settingId);
    Task<IResult> CreateSettingObject(Setting setting);
    Task<IResult> UpdateSettingDetails(Setting update, int settingId);
    Task<IResult> RemoveSettingObject(int settingId);
}

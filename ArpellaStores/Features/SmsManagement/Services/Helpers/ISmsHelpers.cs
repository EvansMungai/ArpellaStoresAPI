namespace ArpellaStores.Features.SmsManagement.Services;

public interface ISmsHelpers
{
    Task<List<string>> GetUsersInRoleAsync(string roleName);
}

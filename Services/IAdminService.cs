using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Services;

public interface IAdminService
{
    Task<List<string>> GetRoles();
    Task<bool> EnsureRoleExists(string role);
    Task<IdentityResult> CreateRole(string role);
    Task<IdentityResult> RemoveRole(string roleName);
}

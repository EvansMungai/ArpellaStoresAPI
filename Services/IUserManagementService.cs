using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Services;

public interface IUserManagementService
{
    Task<IResult> RegisterSpecialUsers(UserManager<User> userManager, User model, string role);
    Task<IResult> GetUsers();
    Task<IResult> GetUserDetails(string number);
    Task<IResult> GetUser(string number);
    Task<IResult> RemoveUser(string number);
    Task<List<string>> GetRoles();
    Task<IResult> AssignRoleToUserAsync(string userId, string roleName);
    Task<bool> EnsureRoleExists(string role);
    Task<IdentityResult> CreateRole(string role);
    Task<IResult> EditRole(string role, string newRoleName);
    Task<IdentityResult> RemoveRole(string roleName);
}

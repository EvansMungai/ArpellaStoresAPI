using ArpellaStores.Features.Authentication.Models;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Features.Authentication.Services;

public interface IUserManagementService
{
    Task<IResult> RegisterSpecialUsers(UserProfile userProfile);
    Task<IResult> GetUsers();
    Task<IResult> GetSpecialUsers();
    Task<IResult> UpdateUserDetails(string number, User update);
    Task<IResult> GetUser(string number);
    Task<IResult> RemoveUser(string number);
    Task<List<string>> GetRoles();
    Task<IResult> AssignRoleToUserAsync(string userId, string roleName);
    Task<bool> EnsureRoleExists(string role);
    Task<IdentityResult> CreateRole(string role);
    Task<IResult> EditRole(string role, string newRoleName);
    Task<IdentityResult> RemoveRole(string roleName);
    Task<IResult> ChangeUserPassword(ChangePasswordModel model, HttpContext context);
}

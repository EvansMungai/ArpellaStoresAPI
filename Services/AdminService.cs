using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public AdminService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._roleManager = roleManager;
    }
    #region Utilities
    public async Task<IdentityResult> AddUserToRole(User model, string role)
    {
        await EnsureRoleExists(role);
        var user = await _userManager.FindByEmailAsync(model.Email);
        return user != null ? await _userManager.AddToRoleAsync(user, role) : IdentityResult.Failed();
    }
    public async Task<List<string>> GetRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleNames = roles.Select(r => r.Name).ToList();
        return roleNames;
    }
    public async Task<bool> EnsureRoleExists(string role)
    {
        return await _roleManager.RoleExistsAsync(role);
    }
    public async Task<IdentityResult> CreateRole(string role)
    {
        bool exists = await EnsureRoleExists(role);
        if (!exists)
        {
            return await _roleManager.CreateAsync(new IdentityRole(role));
        }
        return IdentityResult.Success;
    }
    public async Task<IdentityResult> RemoveRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role != null)
        {
            return await _roleManager.DeleteAsync(role);
        }
        return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' does not exist." });
    }
    #endregion
}

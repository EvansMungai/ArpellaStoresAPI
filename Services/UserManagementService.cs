using ArpellaStores.Data;
using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ArpellaContext _context;
    public UserManagementService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ArpellaContext context)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._roleManager = roleManager;
        this._context = context;
    }
    #region Users 
    public async Task<IResult> GetUsers()
    {
        //List<User> users = await _userManager.Users.ToListAsync();
        //if (users != null || users.Count != 0)
        //{
        //    var userDetails = users.Select(u => new { u.UserName, u.Email, u.PhoneNumber, u.FirstName, u.LastName }).ToList();
        //    return Results.Ok(userDetails);
        //}
        //return Results.NotFound("No users exist");
        var query = from user in _context.Users
                    join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                    join role in _context.Roles on userRoles.RoleId equals role.Id
                    select new
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = role.Name,
                    };
        var result = query.ToList();
        return Results.Ok(result);
    }
    public async Task<IResult> GetUser(string number)
    {
        User user = _userManager.Users.SingleOrDefault(u => u.PhoneNumber == number);
        return user == null ? Results.NotFound($"User with Username = {number} was not found") : Results.Ok(user);
    }
    public async Task<IResult> RemoveUser(string number)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(number);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Results.Ok("User deleted successfully.");
            }
            else
            {
                return Results.BadRequest(result.Errors);
            }
        }
        catch (Exception ex)
        {
            return Results.BadRequest("Exception: " + ex.Message);
        }


    }
    #endregion
    #region Roles
    public async Task<IResult> AssignRoleToUserAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByNameAsync(userId);
        if (user == null)
        {
            return Results.NotFound($"User with Username {userId} not found");
        }
        if (await _userManager.IsInRoleAsync(user, roleName))
        {
            return Results.BadRequest($"User is already in the role {roleName}");
        }
        await _userManager.AddToRoleAsync(user, roleName);
        return Results.Ok($"User with id {userId} has been assigned role = {roleName}");
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
    public async Task<IResult> EditRole(string role, string newRoleName)
    {
        try
        {
            var existingRole = await _roleManager.FindByNameAsync(role);
            if (role == null)
            {
                return Results.NotFound($"The Role with role name {role}");
            }
            existingRole.Name = newRoleName;

            var result = await _roleManager.UpdateAsync(existingRole);
            if (result.Succeeded)
                return Results.Ok(existingRole);
            return Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
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

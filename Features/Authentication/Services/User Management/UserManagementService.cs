using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.Authentication.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ArpellaContext _context;
    public UserManagementService(UserManager<User> userManager,RoleManager<IdentityRole> roleManager, ArpellaContext context)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._context = context;
    }
    #region Users 
    public async Task<IResult> RegisterSpecialUsers(UserProfile userProfile)
    {
        User user = new User
        {
            FirstName = userProfile.User.FirstName,
            LastName = userProfile.User.LastName,
            PhoneNumber = userProfile.User.PhoneNumber,
            UserName = userProfile.User.PhoneNumber,
            Email = userProfile.User.Email,
            PasswordHash = userProfile.User.PasswordHash,
            LastLoginTime = DateTime.Now
        };
        try
        {
            var result = await _userManager.CreateAsync(user, user.PasswordHash);
            if (result.Succeeded)
            {
                await AssignRoleToUserAsync(user.UserName, userProfile.Role);
                return await GetUser(user.UserName);
            }
            return Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.BadRequest("Error occurred!: " + ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> GetUsers()
    {
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
    public async Task<IResult> GetSpecialUsers()
    {
        var excludedRoleId = _context.Roles.Where(role => role.Name == "Customer").Select(role => role.Id).SingleOrDefault();
        var query = from user in _context.Users
                    join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                    join role in _context.Roles on userRoles.RoleId equals role.Id
                    where (userRoles.RoleId != excludedRoleId)
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
        var query = from user in _context.Users
                    join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                    join role in _context.Roles on userRoles.RoleId equals role.Id
                    where user.UserName == number
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
        return result == null || result.Count == 0 ? Results.NotFound($"User with Username = {number} was not found") : Results.Ok(result);
    }
    public async Task<IResult> UpdateUserDetails(string number, User model)
    {
        User retrievedUser = await _userManager.FindByNameAsync(number);
        if (retrievedUser == null)
            return Results.NotFound($"User with Username = {number} was not found");
        retrievedUser.FirstName = model.FirstName;
        retrievedUser.LastName = model.LastName;
        retrievedUser.PhoneNumber = model.PhoneNumber;
        retrievedUser.UserName = model.PhoneNumber;
        retrievedUser.Email = model.Email;
        retrievedUser.PasswordHash = model.PasswordHash;
        
        var result = await _userManager.UpdateAsync(retrievedUser);
        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);
        return Results.Ok(retrievedUser);
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
                return Results.Ok($"User {user} deleted successfully.");
            }
            else
            {
                return Results.BadRequest(result.Errors);
            }
        }
        catch (Exception ex)
        {
            return Results.BadRequest("Exception: " + ex.InnerException?.Message);
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

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return Results.BadRequest("There was an error in removing the user's existing roles");
        }

        var addResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addResult.Succeeded)
        {
            return Results.BadRequest("There was an error in assigning the new role to the user");
        }

        return Results.Ok($"User with Username {userId} has been assigned role = {roleName}");
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

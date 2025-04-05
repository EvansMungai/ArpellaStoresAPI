using ArpellaStores.Data;
using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
//using Microsoft.AspNetCore.Mvc;

namespace ArpellaStores.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserManagementService _userManagementService;
    private readonly ArpellaContext _context;
    private readonly IMemoryCache _cache;
    public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IUserManagementService userManagementService, ArpellaContext context, IMemoryCache cache)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._roleManager = roleManager;
        this._userManagementService = userManagementService;
        this._context = context;
        this._cache = cache;
    }
    public async Task<IResult> RegisterUser(UserManager<User> userManager, User model)
    {
        User user1 = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            UserName = model.PhoneNumber,
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            LastLoginTime = DateTime.Now,
        };
        try
        {
            var result = await _userManager.CreateAsync(user1, user1.PasswordHash);
            if (result.Succeeded)
            {
                var roleAssignmentResult = await _userManagementService.AssignRoleToUserAsync(user1.UserName, "Customer");
                var query = from user in _context.Users
                            join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                            join role in _context.Roles on userRoles.RoleId equals role.Id
                            where (user.UserName == user1.UserName)
                            select new
                            {
                                UserName = user.UserName,
                                Email = user.Email,
                                PhoneNumber = user.PhoneNumber,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = role.Name,
                            };
                var userDetails = query.ToList();
                return Results.Ok(userDetails);
            }
            return Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.BadRequest("Error occurred!: " + ex.InnerException?.Message);
        }
    }
    public async Task<IResult> Login(SignInManager<User> signInManager, UserManager<User> userManager, User model)
    {
        var retrievedUser = await _userManager.FindByNameAsync(model.UserName);
        if (retrievedUser != null)
        {
            var passwordCheck = await _userManager.CheckPasswordAsync(retrievedUser, model.PasswordHash);
            if (passwordCheck)
            {
                retrievedUser.LastLoginTime = DateTime.Now;
                var updateUserResult = await _userManager.UpdateAsync(retrievedUser);
                if (!updateUserResult.Succeeded)
                {
                    return Results.BadRequest("Failed to update LastLoginTime");
                }

                await _signInManager.SignInAsync(retrievedUser, false);

                var query = from user in _context.Users
                            join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                            join role in _context.Roles on userRoles.RoleId equals role.Id
                            where (user.UserName == retrievedUser.UserName)
                            select new
                            {
                                UserName = user.UserName,
                                Email = user.Email,
                                PhoneNumber = user.PhoneNumber,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = role.Name,
                            };
                var userDetails = query.ToList();
                return Results.Ok(userDetails);
            }
            else
            {
                return Results.BadRequest("Incorrect Password.");
            }
        }
        else
        {
            return Results.BadRequest("User not found");
        }
    }
    public async Task<IResult> LogOut(SignInManager<User> signInManager)
    {
        await _signInManager.SignOutAsync();
        return Results.Ok("Successfully logged out");
    }
    #region Utilities
    public async Task<IResult> GetOTP(string username)
    {
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();
        var expiryTime = DateTime.Now.AddMinutes(5);
        _cache.Set(username, (Otp: otp, ExpiryTime: expiryTime), TimeSpan.FromMinutes(5));
        return Results.Ok($"Stored OTP for Username:{username} is {otp}. The OTP expires in 5 minutes.");
    }
    public bool VerifyOTP(string username, string otp, out string message)
    {
        if (_cache.TryGetValue(username, out (string Otp, DateTime ExpiryTime) storedOtpEntry))
        {
            if (storedOtpEntry.Otp == otp)
            {
                message = "OTP is valid";
                return true;
            }
            else
            {
                message = "Invalid OTP";
                return false;
            }
        }
        else
        {
            message = "No OTP found or it has expired";
            return false;
        }
    }

    #endregion
}

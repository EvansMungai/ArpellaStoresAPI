using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ArpellaStores.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserManagementService _userManagementService;
    public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IUserManagementService userManagementService)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._roleManager = roleManager;
        this._userManagementService = userManagementService;
    }
    public async Task<IResult> RegisterUser(UserManager<User> userManager, User model)
    {
        User user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            UserName = model.PhoneNumber,
            Email = model.Email,
            PasswordHash = model.PasswordHash
        };
        try
        {
            var result = await userManager.CreateAsync(user, user.PasswordHash);
            if (result.Succeeded)
            {
                var roleAssignmentResult = await _userManagementService.AssignRoleToUserAsync(user.UserName, "Customer");
                var userDetails = new { user.FirstName, user.LastName, user.PhoneNumber, user.Email };
                return Results.Ok(userDetails);
            }
            return Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.BadRequest("Error occurred!: " + ex.Message);
        }
    }
    public async Task<IResult> Login(SignInManager<User> signInManager, User model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user != null)
        {
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.PasswordHash);
            if (passwordCheck)
            {
                await _signInManager.SignInAsync(user, false);
                var userDetails = new { user.FirstName, user.LastName, user.PhoneNumber, user.Email };
                return Results.Ok(userDetails);
            }
            else
            {
                return Results.BadRequest("Invalid login attempt");
            }
        }
        else
        {
            return Results.BadRequest("User not found");
        }
        //return signInResult.Succeeded ? Results.Ok("You are successfully Logged in") : Results.BadRequest("Error occcured");
    }

}

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
    public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._roleManager = roleManager;
    }
    public async Task<IResult> RegisterUser(UserManager<User> userManager, User model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if(existingUser != null)  return Results.BadRequest("User already exists"); 
        User user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            UserName = model.PhoneNumber,
            Email = model.Email
        };
        try
        {
            var result = await userManager.CreateAsync(user, model.PasswordHash);
            var userDetails = new { user.FirstName, user.LastName, user.PhoneNumber, user.Email };
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Results.Ok(userDetails);
            }
            else
            {
                return Results.BadRequest(result.Errors);
            }
        }
        catch (Exception ex)
        {
            return Results.BadRequest("Error occurred!: " + ex.Message);
        }
    }
    public async Task<IResult> Login(SignInManager<User> signInManager, User model)
    {
        //var signInResult = await _signInManager.PasswordSignInAsync(
        //    userName: userName!,
        //    password: password!,
        //    isPersistent: false,
        //    lockoutOnFailure: false
        //    );
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

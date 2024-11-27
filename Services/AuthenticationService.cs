using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
    }
    public async Task<IResult> RegisterUser(UserManager<User> userManager, User model)
    {
        User user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            UserName = model.PhoneNumber,
            PasswordHash = model.PasswordHash
        };
        try
        {
            var result = await userManager.CreateAsync(user, user.PasswordHash);
            return result.Succeeded? Results.Ok("Registration done successfully") : Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.BadRequest("Error occurred!: " + ex.Message);
        }
    }
    public async Task<IResult> Login(string userName, string password)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(
            userName: userName!,
            password: password!,
            isPersistent: false,
            lockoutOnFailure: false
            );
        return signInResult.Succeeded ? Results.Ok("You are successfully Logged in") : Results.BadRequest("Error occcured");
    }

}

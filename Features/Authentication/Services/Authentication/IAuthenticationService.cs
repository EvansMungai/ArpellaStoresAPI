using ArpellaStores.Features.Authentication.Models;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Features.Authentication.Services.Authentication;

public interface IAuthenticationService
{
    Task<IResult> RegisterUser(UserManager<User> userManager, User model);
    Task<IResult> Login(SignInManager<User> signInManager, UserManager<User> userManager, User model);
    Task<IResult> LogOut(SignInManager<User> signInManager);
    Task<IResult> GetOTP(string username);
    bool VerifyOTP(string username, string otp, out string message);
}

using ArpellaStores.Features.Authentication.Models;

namespace ArpellaStores.Features.Authentication.Services;

public interface IAuthenticationService
{
    Task<IResult> RegisterUser(User model);
    Task<IResult> Login(User model);
    Task<IResult> LogOut();
    Task<IResult> GetOTP(string username);
    bool VerifyOTP(string username, string otp, out string message);
}

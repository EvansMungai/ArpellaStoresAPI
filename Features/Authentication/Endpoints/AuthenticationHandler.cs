using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.Authentication.Models;
using ArpellaStores.Features.Authentication.Services;

namespace ArpellaStores.Features.Authentication.Endpoints;

public class AuthenticationHandler : IHandler
{
    public static string RouteName => "Authentication";
    private readonly IAuthenticationService _authenticationService;
    public AuthenticationHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    public Task<IResult> RegisterUser(User model) => _authenticationService.RegisterUser(model);
    public Task<IResult> Login(User model) => _authenticationService.Login(model);
    public Task<IResult> LogOut() => _authenticationService.LogOut();

    public IResult GetOTP(string username) => _authenticationService.GetOTP(username);

    public IResult VerifyOtp(string username, string otp)
    {
        string message;
        bool isValid = _authenticationService.VerifyOTP(username, otp, out message);
        return Results.Ok(new { Success = isValid, Message = message });
    }
}


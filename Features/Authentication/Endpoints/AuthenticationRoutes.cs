using ArpellaStores.Extensions;
using ArpellaStores.Features.Authentication.Services.Authentication;
using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Features.Authentication.Endpoints;

public class AuthenticationRoutes : IRouteRegistrar
{
    private readonly IAuthenticationService _authenticationService;
    public AuthenticationRoutes(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapAuthenticationRoutes(app);
    }

    public void MapAuthenticationRoutes(WebApplication webApplication)
    {
        webApplication.MapGet("/", () => "Welcome to Arpella Stores Web API!");

        var app = webApplication.MapGroup("").WithTags("Authentication");
        app.MapPost("/register", (UserManager<User> userManager, User model) => _authenticationService.RegisterUser(userManager, model));
        app.MapPost("/login", (SignInManager<User> signInManager, UserManager<User> userManager, User model) => _authenticationService.Login(signInManager, userManager, model));
        app.MapPost("/logout", (SignInManager<User> signInManager) => _authenticationService.LogOut(signInManager));
        app.MapGet("/send-otp", (string username) => _authenticationService.GetOTP(username));
        app.MapPost("/verify-otp", (string username, string otp) =>
        {
            string message;
            bool isValid = _authenticationService.VerifyOTP(username, otp, out message);
            return new
            {
                Success = isValid,
                Message = message
            };
            ;
        });
    }
}

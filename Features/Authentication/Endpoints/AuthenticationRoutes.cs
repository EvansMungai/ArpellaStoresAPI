using ArpellaStores.Extensions;
using ArpellaStores.Features.Authentication.Models;

namespace ArpellaStores.Features.Authentication.Endpoints;

public class AuthenticationRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapAuthenticationRoutes(app);
    }

    public void MapAuthenticationRoutes(WebApplication webApplication)
    {
        webApplication.MapGet("/", () => "Welcome to Arpella Stores Web API!");

        var app = webApplication.MapGroup("").WithTags("Authentication");
        app.MapPost("/register", (User model, AuthenticationHandler handler) => handler.RegisterUser(model));
        app.MapPost("/login", (User model, AuthenticationHandler handler) => handler.Login(model));
        app.MapPost("/logout", (AuthenticationHandler handler) => handler.LogOut());
        app.MapGet("/send-otp", (string username, AuthenticationHandler handler) => handler.GetOTP(username));
        app.MapPost("/verify-otp", (string username, string otp, AuthenticationHandler handler) => handler.VerifyOtp(username, otp));
    }
}

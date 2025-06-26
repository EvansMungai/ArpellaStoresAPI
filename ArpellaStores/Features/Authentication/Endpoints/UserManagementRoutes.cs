using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.Authentication.Models;

namespace ArpellaStores.Features.Authentication.Endpoints;

public class UserManagementRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapUserManagementRoutes(app);
        MapRoleManagmentRoutes(app);
    }

    public void MapUserManagementRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("User Management");
        app.MapPost("/control", (UserManagementHandler handler, UserProfile specialUser) => handler.RegisterSpecialUsers(specialUser));
        app.MapGet("/users", (UserManagementHandler handler) => handler.GetUsers());
        app.MapGet("/user/{id}", (UserManagementHandler handler, string id) => handler.GetUser(id));
        app.MapGet("/special-users", (UserManagementHandler handler) => handler.GetSpecialUsers());
        app.MapDelete("/user/{id}", (UserManagementHandler handler, string id) => handler.RemoveUser(id));
        app.MapPut("/user-role/{id}", (UserManagementHandler handler, string id, string role) => handler.AssignRoleToUserAsync(id, role));
        app.MapPut("/user-details/{id}", (UserManagementHandler handler, string id, User update) => handler.UpdateUserDetails(id, update));

    }
    public void MapRoleManagmentRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Role Management");
        app.MapGet("/roles", (UserManagementHandler handler) => handler.GetRoles());
        app.MapGet("/role/{roleName}", (UserManagementHandler handler, string roleName) => handler.EnsureRoleExists(roleName));
        app.MapPost("/role", (UserManagementHandler handler, string role) => handler.CreateRole(role));
        app.MapPut("/role/{roleName}", (UserManagementHandler handler, string roleName, string newRoleName) => handler.EditRole(roleName, newRoleName));
        app.MapDelete("/role/{roleName}", (UserManagementHandler handler, string roleName) => handler.RemoveRole(roleName));
    }
}

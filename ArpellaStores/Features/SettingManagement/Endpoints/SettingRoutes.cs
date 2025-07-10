using ArpellaStores.Extensions;
using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.OrderManagement.Endpoints;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.SettingManagement.Models;
using System.Dynamic;

namespace ArpellaStores.Features.SettingManagement.Endpoints;

public class SettingRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapSettingRoutes(app);
    }
    public void MapSettingRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Setting");
        app.MapGet("/settings", (SettingHandler handler) => handler.GetSettings()).Produces(200).Produces(404).Produces<List<Setting>>();
        app.MapGet("/setting/{id}", (SettingHandler handler, int id) => handler.GetSetting(id)).Produces(200).Produces(404).Produces<Setting>();
        app.MapPost("/setting", (SettingHandler handler, Setting setting) => handler.CreateSetting(setting)).Produces(202).Produces(404).Produces(400).Produces<Setting>().AddEndpointFilter<ValidationEndpointFilter<Setting>>();
        app.MapPut("/setting/{id}", (SettingHandler handler, Setting setting, int id) => handler.UpdateSettingDetails(setting, id)).Produces(200).Produces(404).Produces(400).Produces<Setting>().AddEndpointFilter<ValidationEndpointFilter<Setting>>();
        app.MapDelete("/setting/{id}", (SettingHandler handler, int id) => handler.RemoveSettingObject(id)).Produces(200).Produces(404).Produces<Order>();
    }
}

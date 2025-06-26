using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.InventoryManagement.Models;


namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class InventoryRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapInventoryRoutes(app);
    }
    public void MapInventoryRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Inventories");
        app.MapGet("/inventories", (InventoryHandler handler) => handler.GetInventories()).Produces(200).Produces(404).Produces<List<Inventory>>();
        app.MapGet("/inventory/{id}", (InventoryHandler handler ,string id) => handler.GetInventory(id)).Produces(200).Produces(404).Produces<Inventory>();
        app.MapPost("/inventory", (InventoryHandler handler, Inventory  inventory) => handler.CreateInventory(inventory)).Produces(200).Produces(404).Produces<Inventory>();
        app.MapPost("/inventories", (InventoryHandler handler, IFormFile file) => handler.CreateInventories(file)).Produces(200).Produces(404).Produces<List<Inventory>>().DisableAntiforgery();
        app.MapPut("/inventory/{id}", (InventoryHandler handler, Inventory inventory, string id) => handler.UpdateInventory(inventory, id)).Produces(200).Produces(404).Produces<Inventory>();
        app.MapDelete("/inventory/{id}", (InventoryHandler handler, string id) => handler.RemoveInventory(id)).Produces(200).Produces(404).Produces<Inventory>();
        app.MapGet("/inventorylevels", (InventoryHandler handler) => handler.CheckInventoryLevels()).Produces(200).Produces(404).Produces<List<Inventory>>();
    }
}

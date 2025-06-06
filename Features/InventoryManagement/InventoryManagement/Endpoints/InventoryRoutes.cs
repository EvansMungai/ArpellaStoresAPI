using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.InventoryManagement.Services;


namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class InventoryRoutes : IRouteRegistrar
{
    private readonly IInventoryService _inventoryService;
    public InventoryRoutes(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapInventoryRoutes(app);
    }
    public void MapInventoryRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Inventories");
        app.MapGet("/inventories", () => this._inventoryService.GetInventories()).Produces(200).Produces(404).Produces<List<Inventory>>();
        app.MapGet("/inventory/{id}", (string id) => this._inventoryService.GetInventory(id)).Produces(200).Produces(404).Produces<Inventory>();
        app.MapPost("/inventory", (Inventory inventory) => this._inventoryService.CreateInventory(inventory)).Produces(200).Produces(404).Produces<Inventory>();
        app.MapPost("/inventories", (IFormFile file) => this._inventoryService.CreateInventories(file)).Produces(200).Produces(404).Produces<List<Inventory>>().DisableAntiforgery();
        app.MapPut("/inventory/{id}", (Inventory inventory, string id) => this._inventoryService.UpdateInventory(inventory, id)).Produces(200).Produces(404).Produces<Inventory>();
        app.MapDelete("/inventory/{id}", (string id) => this._inventoryService.RemoveInventory(id)).Produces(200).Produces(404).Produces<Inventory>();
        app.MapGet("/inventorylevels", () => this._inventoryService.CheckInventoryLevels()).Produces(200).Produces(404).Produces<List<Inventory>>();
    }
}

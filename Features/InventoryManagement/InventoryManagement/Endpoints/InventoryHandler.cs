using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.InventoryManagement.Services;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class InventoryHandler : IHandler
{
    public static string RouteName => "Inventory Management";
    private readonly IInventoryService _inventoryService;

    public InventoryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public Task<IResult> GetInventories() => _inventoryService.GetInventories();
    public Task<IResult> GetInventory(string id) => _inventoryService.GetInventory(id);
    public Task<IResult> CreateInventory(Inventory inventory) => _inventoryService.CreateInventory(inventory);
    public Task<IResult> CreateInventories(IFormFile file) => _inventoryService.CreateInventories(file);
    public Task<IResult> UpdateInventory(Inventory update, string id) => _inventoryService.UpdateInventory(update, id);
    public Task<IResult> RemoveInventory(string id) => _inventoryService.RemoveInventory(id);
    public Task<IResult> CheckInventoryLevels() => _inventoryService.CheckInventoryLevels();
    public Task<IResult> RestockInventory(Restocklog restocklog) => _inventoryService.RestockInventory(restocklog);
    public Task<IResult> GetRestockLogs() => _inventoryService.GetRestockLogs();
}

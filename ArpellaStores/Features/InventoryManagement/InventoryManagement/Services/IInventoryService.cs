using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IInventoryService
{
    Task<IResult> GetInventories();
    Task<IResult> GetPagedInventories(int pageNumber, int pageSize);
    Task<IResult> GetInventory(string id);
    Task<IResult> CreateInventory(Inventory inventory);
    Task<IResult> CreateInventories(IFormFile file);
    Task<IResult> UpdateInventory(Inventory update, string id);
    Task<IResult> RemoveInventory(string id);
    Task<IResult> CheckInventoryLevels();
    Task<IResult> RestockInventory(Restocklog restocklog);
    Task<IResult> GetRestockLogs();
}

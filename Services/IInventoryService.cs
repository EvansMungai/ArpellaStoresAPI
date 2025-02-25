using ArpellaStores.Models;

namespace ArpellaStores.Services;

public interface IInventoryService
{
    Task<IResult> GetInventories();
    Task<IResult> GetInventory(string id);
    Task<IResult> CreateInventory(Inventory inventory);
    Task<IResult> UpdateInventory(Inventory update, string id);
    Task<IResult> RemoveInventory(string id);
}

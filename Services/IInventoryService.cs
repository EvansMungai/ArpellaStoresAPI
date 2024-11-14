using ArpellaStores.Models;

namespace ArpellaStores.Services;

public interface IInventoryService
{
    Task<IResult> GetInventories();
    Task<IResult> GetInventory(int id);
    Task<IResult> CreateInventory(Inventory inventory);
    Task<IResult> UpdateInventory(Inventory update, int id);
    Task<IResult> RemoveInventory(int id);
}

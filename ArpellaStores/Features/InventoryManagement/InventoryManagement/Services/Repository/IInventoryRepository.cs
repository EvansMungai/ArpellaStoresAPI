using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IInventoryRepository
{
    Task<List<Inventory>> GetAllInventoriesAsync();
    Task<Inventory?> GetInventoryByIdAsync(string id);
    Task AddInventoryAsync(Inventory inventory);
    Task<int> AddInventoriesAsync(List<Inventory> inventory);
    Task UpdateInventoryDetails(Inventory inventory, string id);
    Task RemoveInventoryAsync(string id);
    Task<List<Inventory>> CheckInventoryLevelsAsync();
    Task RestockInventoryAsync(Restocklog restocklog);
    Task<List<Restocklog>> GetAllRestocklogsAsync();
}

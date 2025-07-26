using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface ISupplierRepository
{
    Task<List<Supplier>> GetAllSuppliersAsync();
    Task<Supplier?> GetSupplierByIdAsync(int id);
    Task AddSupplierAsync(Supplier supplier);
    Task<bool> UpdateSupplierDetailsAsync(Supplier update, int id);
    Task<bool> RemoveSupplierAsync(int id);
}

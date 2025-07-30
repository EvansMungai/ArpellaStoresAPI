using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IProductImageRepository
{
    Task<List<Productimage>> GetProductImageDetailsAsync();
    Task<List<Productimage>> GetProductImageUrlAsync(int productId);
    Task<Productimage?> GetProductImageObjectById(int id);
    Task AddProductImageDetails(Productimage productimage);
    Task RemoveProductImage(int id);
}

using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface ISubcategoryRepository
{
    Task<List<Subcategory>> GetAllSubcategoriesAsync();
    Task<Subcategory?> GetSubcategoryByIdAsync(int id);
    Task AddSubcategoryAsync(Subcategory subcategory);
    Task UpdateSubcategoryDetailsAsync(Subcategory subcategory, int id);
    Task RemoveSubcategoryAsync(int id);
}

using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface ISubcategoriesServices
{
    Task<IResult> GetSubcategories();
    Task<IResult> GetSubcategory(int id);
    Task<IResult> CreateSubcategory(Subcategory subcategory);
    Task<IResult> UpdateSubcategoryDetails(Subcategory update, int id);
    Task<IResult> RemoveSubcategory(int id);
}

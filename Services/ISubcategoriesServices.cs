using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public interface ISubcategoriesServices
    {
        Task<IResult> GetSubcategories();
        Task<IResult> GetSubcategory(int id);
        Task<IResult> CreateSubcategory(Subcategory subcategory);
        Task<IResult> UpdateSubcategoryDetails(Subcategory update, int id);
        Task<IResult> RemoveSubcategory(int id);
    }
}

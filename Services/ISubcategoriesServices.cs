using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public interface ISubcategoriesServices
    {
        Task<IResult> GetSubcategories();
        Task<IResult> GetSubcategory(string id);
        Task<IResult> CreateSubcategory(Subcategory subcategory);
        Task<IResult> UpdateSubcategoryDetails(Subcategory update, string id);
        Task<IResult> RemoveSubcategory(string id);
    }
}

using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public interface ISubcategoriesServices
    {
        Task<List<Subcategory>> GetSubcategories();
        Subcategory CreateSubcategory(Subcategory subcategory);
        Subcategory? RemoveSubcategory(string id);
    }
}

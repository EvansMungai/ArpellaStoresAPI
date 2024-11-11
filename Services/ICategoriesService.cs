using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public interface ICategoriesService
    {
        Task<IResult> GetCategories();
        Task<IResult> GetCategory(string id);
        Task<IResult> CreateCategory(Category category);
        Task<IResult> UpdateCategoryDetails(Category update, string id);
        Task<IResult> RemoveCategory(string categoryId);
    }
}

using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public interface ICategoriesService
    {
        Task<IResult> GetCategories();
        Task<IResult> GetCategory(int id);
        Task<IResult> CreateCategory(Category category);
        Task<IResult> UpdateCategoryDetails(Category update, int id);
        Task<IResult> RemoveCategory(int categoryId);
    }
}

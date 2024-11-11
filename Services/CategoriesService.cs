using ArpellaStores.Data;
using ArpellaStores.Models;
using EntityFramework.Exceptions.Common;

namespace ArpellaStores.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ArpellaContext _context;
        public CategoriesService(ArpellaContext context)
        {
            _context = context;
        }
        public async Task<IResult> GetCategories()
        {
            var categories = _context.Categories.ToList();
            if (categories == null || categories.Count == 0)
            {
                return Results.NotFound("No Categories found");
            }
            else
            {
                return Results.Ok(categories);
            }
        }
        public async Task<IResult> GetCategory(string id)
        {
            Category? category = _context.Categories.SingleOrDefault(c => c.Id == id);
            return category == null ? Results.NotFound($"Category with CategoryID = {id} was not found") : Results.Ok(category);
        }
        public async Task<IResult> CreateCategory(Category category)
        {
            var newCategory = new Category
            {
                Id = category.Id,
                CategoryName = category.CategoryName
            };
            try
            {
                _context.Categories.Add(newCategory);
                await _context.SaveChangesAsync();
            }
            catch (UniqueConstraintException ex)
            {

            }
            return Results.Ok(newCategory);
        }
        public async Task<IResult> UpdateCategoryDetails(Category update, string id)
        {
            var retrievedCategory = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (retrievedCategory != null)
            {
                retrievedCategory.Id = update.Id;
                retrievedCategory.CategoryName = update.CategoryName;
                try
                {
                    _context.Update(retrievedCategory);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
                return Results.Ok(retrievedCategory);
            }
            else
            {
                return Results.NotFound($"Category with CategoryID = {id} was not found");
            }

        }
        public async Task<IResult> RemoveCategory(string categoryId)
        {
            var retrievedCategory = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (retrievedCategory != null)
            {
                _context.Categories.Remove(retrievedCategory);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedCategory);
            }
            else
            {
                return Results.NotFound($"Category with CategoryID = {categoryId} was not found");
            }
        }
    }
}

using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class CategoryRepository : ICategoryRepository
{
    private readonly ArpellaContext _context;
    public CategoryRepository(ArpellaContext context)
    {
        _context = context;
    }

    public async Task AddCategoryAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.Select(c => new Category{ Id = c.Id, CategoryName = c.CategoryName }).AsNoTracking().ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.Select(c => new Category { Id= c.Id, CategoryName = c.CategoryName }).AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task RemoveCategoryAsync(int id)
    {
        Category? retrievedCategory = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);

        if (retrievedCategory == null) return;

        _context.Categories.Remove(retrievedCategory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategoryDetailsAsync(Category update, int id)
    {
        var retrievedCategory = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);
        if (retrievedCategory == null) return;

        retrievedCategory.CategoryName = update.CategoryName;
        _context.Categories.Update(retrievedCategory);
        await _context.SaveChangesAsync();
    }
}

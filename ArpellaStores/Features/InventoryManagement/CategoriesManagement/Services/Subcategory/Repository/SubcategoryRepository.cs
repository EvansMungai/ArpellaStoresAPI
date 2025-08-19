using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class SubcategoryRepository : ISubcategoryRepository
{
    private readonly ArpellaContext _context;
    public SubcategoryRepository(ArpellaContext context)
    {
        _context = context;
    }

    public async Task AddSubcategoryAsync(Subcategory subcategory)
    {
        _context.Subcategories.Add(subcategory);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Subcategory>> GetAllSubcategoriesAsync()
    {
        return await _context.Subcategories.Select(s => new Subcategory { Id = s.Id, SubcategoryName = s.SubcategoryName, CategoryId = s.CategoryId }).AsNoTracking().ToListAsync();
    }

    public async Task<Subcategory?> GetSubcategoryByIdAsync(int id)
    {
        return await _context.Subcategories.Select(s => new Subcategory { Id = s.Id, SubcategoryName = s.SubcategoryName, CategoryId = s.CategoryId }).AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task RemoveSubcategoryAsync(int id)
    {
        Subcategory? retrievedSubcategory = await _context.Subcategories.SingleOrDefaultAsync(s => s.Id == id);

        if (retrievedSubcategory == null) return;

        _context.Subcategories.Remove(retrievedSubcategory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSubcategoryDetailsAsync(Subcategory subcategory, int id)
    {
        var retrievedSubcategory = await _context.Subcategories.SingleOrDefaultAsync(s => s.Id == id);

        if (retrievedSubcategory == null) return;

        retrievedSubcategory.SubcategoryName = subcategory.SubcategoryName;
        retrievedSubcategory.CategoryId = subcategory.CategoryId;
        _context.Subcategories.Update(retrievedSubcategory);
        await _context.SaveChangesAsync();
    }
}

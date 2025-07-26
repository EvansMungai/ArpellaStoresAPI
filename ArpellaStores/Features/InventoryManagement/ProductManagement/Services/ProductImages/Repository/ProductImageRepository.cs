using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ArpellaContext _context;
    public ProductImageRepository(ArpellaContext context)
    {
        _context = context;
    }

    public async Task AddProductImageDetails(Productimage productimage)
    {
        _context.Productimages.Add(productimage);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Productimage>> GetProductImageDetailsAsync()
    {
        return await _context.Productimages.AsNoTracking().ToListAsync();
    }

    public async Task<Productimage?> GetProductImageObjectById(int id)
    {
        return await _context.Productimages.FindAsync(id);
    }

    public async Task<List<Productimage>> GetProductImageUrlAsync(string productId)
    {
        return await _context.Productimages.AsNoTracking().Where(i => i.ProductId == productId).ToListAsync();
    }

    public async Task RemoveProductImage(int id)
    {
        var existingProductImage = await _context.Productimages.FindAsync(id);
        if (existingProductImage == null) return;

        _context.Productimages.Remove(existingProductImage);
        await _context.SaveChangesAsync();
    }
}

using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class ProductRepository : IProductRepository
{
    private readonly ArpellaContext _context;
    public ProductRepository(ArpellaContext context)
    {
        _context = context;
    }

    public async Task AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task AddProductsAsync(List<Product> products)
    {
        foreach (var batch in products.Chunk(500))
        {
            _context.Products.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.Select(p => new Product { Id = p.Id, InventoryId = p.InventoryId, Name = p.Name, Price = p.Price, PriceAfterDiscount = p.PriceAfterDiscount, Category = p.Category, PurchaseCap = p.PurchaseCap, Subcategory = p.Subcategory, Barcodes = p.Barcodes, DiscountQuantity = p.DiscountQuantity, ShowOnline = p.ShowOnline, CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt }).AsNoTracking().ToListAsync();
    }

    public async Task<List<Product>> GetPagedProductsAsync(int pageNumber, int pageSize)
    {
        return await _context.Products.Where(p => p.ShowOnline == true).Select(p => new Product { Id = p.Id, InventoryId = p.InventoryId, Name = p.Name, Price = p.Price, PriceAfterDiscount = p.PriceAfterDiscount, Category = p.Category, PurchaseCap = p.PurchaseCap, Subcategory = p.Subcategory, Barcodes = p.Barcodes, DiscountQuantity = p.DiscountQuantity, ShowOnline = p.ShowOnline, CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt })
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .AsNoTracking().ToListAsync();
    }
    public async Task<List<Product>> GetPagedPOSProductsAsync(int pageNumber, int pageSize)
    {
        return await _context.Products.Select(p => new Product { Id = p.Id, InventoryId = p.InventoryId, Name = p.Name, Price = p.Price, PriceAfterDiscount = p.PriceAfterDiscount, Category = p.Category, PurchaseCap = p.PurchaseCap, Subcategory = p.Subcategory, Barcodes = p.Barcodes, DiscountQuantity = p.DiscountQuantity, ShowOnline = p.ShowOnline, CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt })
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .AsNoTracking().ToListAsync();
    }
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.Select(p => new Product { Id = p.Id, InventoryId = p.InventoryId, Name = p.Name, Price = p.Price, PriceAfterDiscount = p.PriceAfterDiscount, Category = p.Category, PurchaseCap = p.PurchaseCap, Subcategory = p.Subcategory, Barcodes = p.Barcodes, DiscountQuantity = p.DiscountQuantity, ShowOnline = p.ShowOnline, CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt }).AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task RemoveProduct(int id)
    {
        var retrievedProduct = await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
        if (retrievedProduct == null) return;

        _context.Products.Remove(retrievedProduct);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProductDetails(Product product, int id)
    {
        var retrievedProduct = await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
        if (retrievedProduct == null) return;

        retrievedProduct.Name = product.Name;
        retrievedProduct.Price = product.Price;
        retrievedProduct.Category = product.Category;
        retrievedProduct.Subcategory = product.Subcategory;
        retrievedProduct.DiscountQuantity = product.DiscountQuantity;
        retrievedProduct.Barcodes = product.Barcodes;
        retrievedProduct.PurchaseCap = product.PurchaseCap;
        retrievedProduct.PriceAfterDiscount = product.PriceAfterDiscount;
        retrievedProduct.ShowOnline = product.ShowOnline;

        _context.Products.Update(retrievedProduct);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateProductPrice(int id, decimal price)
    {
        var retrievedProduct = await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
        if (retrievedProduct == null) return;

        retrievedProduct.PriceAfterDiscount = price;
        _context.Products.Update(retrievedProduct);
        await _context.SaveChangesAsync();
    }
}

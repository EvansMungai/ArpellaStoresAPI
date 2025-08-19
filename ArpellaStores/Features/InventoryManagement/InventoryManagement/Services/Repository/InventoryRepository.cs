using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class InventoryRepository : IInventoryRepository
{
    private readonly ArpellaContext _context;
    public InventoryRepository(ArpellaContext context)
    {
        _context = context;
    }

    public async Task<int> AddInventoriesAsync(List<Inventory> inventory)
    {
        foreach (var batch in inventory.Chunk(500))
        {
            _context.Inventories.AddRangeAsync(batch);
        }
        return await _context.SaveChangesAsync();
    } 

    public async Task AddInventoryAsync(Inventory inventory)
    {
        _context.Inventories.Add(inventory);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Inventory>> CheckInventoryLevelsAsync()
    {
        return await _context.Inventories.Where(i => i.StockQuantity <= i.StockThreshold).ToListAsync();
    }

    public async Task<List<Inventory>> GetAllInventoriesAsync()
    {
        return await _context.Inventories.Select(i => new Inventory { InventoryId = i.InventoryId, ProductId = i.ProductId, StockQuantity = i.StockQuantity, StockThreshold = i.StockThreshold, StockPrice = i.StockPrice, SupplierId = i.SupplierId, InvoiceNumber = i.InvoiceNumber, CreatedAt = i.CreatedAt, UpdatedAt = i.UpdatedAt }).AsNoTracking().ToListAsync();
    }

    public async Task<List<Inventory>> GetPagedInventoriesAsync(int pageNumber, int pageSize)
    {
        return await _context.Inventories.Select(i => new Inventory { InventoryId = i.InventoryId, ProductId = i.ProductId, StockQuantity = i.StockQuantity, StockThreshold = i.StockThreshold, StockPrice = i.StockPrice, SupplierId = i.SupplierId, InvoiceNumber = i.InvoiceNumber, CreatedAt = i.CreatedAt, UpdatedAt = i.UpdatedAt })
            .Skip((pageNumber - 1)* pageSize).Take(pageSize)
            .AsNoTracking().ToListAsync();
    }

    public async Task<List<Restocklog>> GetAllRestocklogsAsync()
    {
        return await _context.Restocklogs.Select(l => new Restocklog { LogId = l.LogId, ProductId = l.ProductId, RestockQuantity = l.RestockQuantity, RestockDate = l.RestockDate, SupplierId = l.SupplierId, InvoiceNumber = l.InvoiceNumber }).ToListAsync();
    }

    public async Task<Inventory?> GetInventoryByIdAsync(string id)
    {
        return await _context.Inventories.Where(i => i.ProductId == id).Select(i => new Inventory { InventoryId = i.InventoryId, ProductId = i.ProductId, StockQuantity = i.StockQuantity, StockThreshold = i.StockThreshold, StockPrice = i.StockPrice, SupplierId = i.SupplierId, InvoiceNumber = i.InvoiceNumber, CreatedAt = i.CreatedAt, UpdatedAt = i.UpdatedAt }).AsNoTracking().SingleOrDefaultAsync();
    }

    public async Task RemoveInventoryAsync(string id)
    {
        Inventory? retrievedInventory = await _context.Inventories.AsNoTracking().SingleOrDefaultAsync(i => i.ProductId == id);
        if (retrievedInventory == null) return;

        _context.Inventories.Remove(retrievedInventory);
        await _context.SaveChangesAsync();
    }

    public async Task RestockInventoryAsync(Restocklog restocklog)
    {
        Inventory? inventory = await _context.Inventories.SingleOrDefaultAsync(i => i.ProductId == restocklog.ProductId);
        if (inventory == null) return;

        inventory.StockQuantity += restocklog.RestockQuantity;

        if (restocklog.PurchasePrice.HasValue)
        {
            inventory.StockPrice = restocklog.PurchasePrice.Value;
        }

        inventory.SupplierId = restocklog.SupplierId;
        inventory.InvoiceNumber = restocklog.InvoiceNumber;
        inventory.UpdatedAt = DateTime.Now;

        _context.Restocklogs.Add(restocklog);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateInventoryDetails(Inventory inventory, string id)
    {
        Inventory? retrievedInventory = await _context.Inventories.AsNoTracking().SingleOrDefaultAsync(i => i.ProductId == id);
        if (retrievedInventory == null) return;

        retrievedInventory.StockQuantity = inventory.StockQuantity;
        retrievedInventory.StockThreshold = inventory.StockThreshold;
        retrievedInventory.UpdatedAt = DateTime.Now;
        retrievedInventory.SupplierId = inventory.SupplierId;

        _context.Inventories.Update(retrievedInventory);
        await _context.SaveChangesAsync();
    }
}

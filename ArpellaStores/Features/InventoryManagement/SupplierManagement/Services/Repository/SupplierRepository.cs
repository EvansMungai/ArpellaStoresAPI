using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class SupplierRepository : ISupplierRepository
{
    private readonly ArpellaContext _context;
    public SupplierRepository(ArpellaContext context)
    {
        _context = context;
    }

    public async Task AddSupplierAsync(Supplier supplier)
    {
        var newSupplier = new Supplier
        {
            SupplierName = supplier.SupplierName,
            KraPin = supplier.KraPin,
        };

        _context.Suppliers.Add(newSupplier);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        return await _context.Suppliers.Select(s => new Supplier { Id = s.Id, SupplierName = s.SupplierName, KraPin = s.KraPin }).AsNoTracking().ToListAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int id)
    {
        return await _context.Suppliers.Select(s => new Supplier { Id = s.Id, SupplierName = s.SupplierName, KraPin = s.KraPin }).AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> RemoveSupplierAsync(int id)
    {
        var retrievedSupplier = await _context.Suppliers.SingleOrDefaultAsync(s => s.Id == id);
        if (retrievedSupplier == null) return false;

        _context.Suppliers.Remove(retrievedSupplier);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSupplierDetailsAsync(Supplier update, int id)
    {
        var retrievedSupplier = await _context.Suppliers.SingleOrDefaultAsync(s => s.Id == id);
        if (retrievedSupplier == null) return false;

        retrievedSupplier.SupplierName = update.SupplierName;
        retrievedSupplier.KraPin = update.KraPin;

        _context.Suppliers.Update(retrievedSupplier);
        await _context.SaveChangesAsync();
        return true;
    }
}

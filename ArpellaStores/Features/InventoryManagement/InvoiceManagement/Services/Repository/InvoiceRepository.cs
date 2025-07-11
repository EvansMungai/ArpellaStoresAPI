using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ArpellaContext _context;
    public InvoiceRepository(ArpellaContext context)
    {
        _context = context;
    }

    public async Task AddInvoice(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Invoice>> GetAllInvoicesAsync()
    {
        return await _context.Invoices.Select(i => new Invoice { InvoiceId = i.InvoiceId, SupplierId = i.SupplierId, TotalAmount = i.TotalAmount, CreatedAt = i.CreatedAt, UpdatedAt = i.UpdatedAt }).AsNoTracking().ToListAsync();
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(string id)
    {
        return await _context.Invoices.Where(i => i.InvoiceId == id).Select(i => new Invoice { InvoiceId = i.InvoiceId, SupplierId = i.SupplierId, TotalAmount = i.TotalAmount, CreatedAt = i.CreatedAt, UpdatedAt = i.UpdatedAt }).AsNoTracking().SingleOrDefaultAsync();
    }

    public async Task RemoveInvoiceAsync(string id)
    {
        Invoice? retrievedInvoice = await _context.Invoices.AsNoTracking().SingleOrDefaultAsync(i => i.InvoiceId == id);
        if (retrievedInvoice == null) return;

        _context.Invoices.Remove(retrievedInvoice);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateInvoiceDetailsAsync(Invoice update, string id)
    {
        Invoice? retrievedInvoice = await _context.Invoices.AsNoTracking().SingleOrDefaultAsync(i => i.InvoiceId == id);
        if (retrievedInvoice == null) return;

        retrievedInvoice.SupplierId = update.SupplierId;
        retrievedInvoice.TotalAmount = update.TotalAmount;
        _context.Invoices.Update(retrievedInvoice);
        await _context.SaveChangesAsync();
    }
}

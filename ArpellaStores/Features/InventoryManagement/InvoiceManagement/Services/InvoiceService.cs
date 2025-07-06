using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class InvoiceService : IInvoiceService
{
    private readonly ArpellaContext _context;
    public InvoiceService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetInvoices()
    {
        var invoices = await _context.Invoices.Select(i => new { i.InvoiceId, i.SupplierId, i.TotalAmount, i.CreatedAt, i.UpdatedAt }).AsNoTracking().ToListAsync();
        return invoices == null || invoices.Count == 0 ? Results.NotFound("No invoices were found") : Results.Ok(invoices);
    }
    public async Task<IResult> GetInvoice(string invoiceId)
    {
        var invoice = await _context.Invoices.Select(i => new { i.InvoiceId, i.SupplierId, i.TotalAmount, i.CreatedAt, i.UpdatedAt }).AsNoTracking().SingleOrDefaultAsync();
        return invoice == null ? Results.NotFound($"Invoice with invoice id = {invoiceId} was not found") : Results.Ok(invoice);
    }
    public async Task<IResult> CreateInvoice(Invoice invoice)
    {
        var local = _context.Invoices.Local.FirstOrDefault(i => i.InvoiceId == invoice.InvoiceId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        // Optional: Check if it already exists in the database
        var existing = await _context.Invoices.AsNoTracking().SingleOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);
        if (existing != null)
            return Results.Conflict($"An invoice with id= {invoice.InvoiceId} already exists.");

        var newInvoice = new Invoice
        {
            InvoiceId = invoice.InvoiceId,
            SupplierId = invoice.SupplierId,
            TotalAmount = invoice.TotalAmount
        };
        try
        {
            _context.Invoices.Add(newInvoice);
            await _context.SaveChangesAsync();
            return Results.Ok(newInvoice);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> UpdateInvoiceDetails(Invoice update, string invoiceId)
    {
        var local = _context.Invoices.Local.FirstOrDefault(i => i.InvoiceId == invoiceId);

        if (local != null)
            _context.Entry(local).State = EntityState.Detached;

        var retrievedInvoice = await _context.Invoices.AsNoTracking().SingleOrDefaultAsync(i => i.InvoiceId == invoiceId);
        if (retrievedInvoice != null)
        {
            retrievedInvoice.SupplierId = update.SupplierId;
            retrievedInvoice.TotalAmount = update.TotalAmount;
            try
            {
                _context.Invoices.Update(retrievedInvoice);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedInvoice);
            }
            catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
        }
        else
        {
            return Results.NotFound($"Invoice with invoice id ={invoiceId} was not found");
        }
    }
    public async Task<IResult> RemoveInvoice(string invoiceId)
    {
        var local = _context.Invoices.Local.FirstOrDefault(i => i.InvoiceId == invoiceId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var retrievedInvoice = await _context.Invoices.AsNoTracking().SingleOrDefaultAsync(i => i.InvoiceId == invoiceId);
        if (retrievedInvoice != null)
        {
            try
            {
                _context.Invoices.Remove(retrievedInvoice);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedInvoice);
            }
            catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
        }
        else { return Results.NotFound($"Invoice with invoice id = {invoiceId} was not found"); }

    }
}

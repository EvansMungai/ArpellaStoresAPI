using ArpellaStores.Data;
using ArpellaStores.Models;

namespace ArpellaStores.Services;

public class InvoiceService : IInvoiceService
{
    private readonly ArpellaContext _context;
    public InvoiceService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetInvoices()
    {
        var invoices = _context.Invoices.Select(i => new { i.InvoiceId, i.SupplierId, i.TotalAmount, i.CreatedAt, i.UpdatedAt }).ToList();
        if (invoices == null || invoices.Count == 0)
        {
            return Results.NotFound("No Invoices found");
        }
        else
        {
            return Results.Ok(invoices);
        }
    }
    public async Task<IResult> GetInvoice(string id)
    {
        var invoice = _context.Invoices.Select(i => new { i.InvoiceId, i.SupplierId, i.TotalAmount, i.CreatedAt, i.UpdatedAt }).SingleOrDefault(i => i.InvoiceId == id);
        return invoice == null ? Results.NotFound($"Invoice with InvoiceID = {id} was not found") : Results.Ok(invoice);
    }
    public async Task<IResult> CreateInvoice(Invoice invoice)
    {
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
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        return Results.Ok(newInvoice);
    }
    public async Task<IResult> UpdateInvoceDetails(Invoice update, string id)
    {
        var retrievedInvoice = _context.Invoices.FirstOrDefault(i => i.InvoiceId == id);
        if (retrievedInvoice != null)
        {
            retrievedInvoice.InvoiceId= update.InvoiceId;
            retrievedInvoice.SupplierId= update.SupplierId;
            retrievedInvoice.TotalAmount= update.TotalAmount;
            try
            {
                _context.Invoices.Update(retrievedInvoice);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
            return Results.Ok(retrievedInvoice);
        }
        else
        {
            return Results.NotFound($"Invoice with InvoiceId = {id} was not found");
        }

    }
    public async Task<IResult> RemoveInvoice(string invoiceId)
    {
        var retrievedInvoice = _context.Invoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
        if (retrievedInvoice != null)
        {
            _context.Invoices.Remove(retrievedInvoice);
            await _context.SaveChangesAsync();
            return Results.Ok(retrievedInvoice);
        }
        else
        {
            return Results.NotFound($"Invoice with InvoiceId = {invoiceId} was not found");
        }
    }
}

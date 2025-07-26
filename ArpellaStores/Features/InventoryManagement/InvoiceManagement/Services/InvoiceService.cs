using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repo;
    public InvoiceService(IInvoiceRepository repo)
    {
        _repo = repo;
    }
    public async Task<IResult> GetInvoices()
    {
        var invoices = await _repo.GetAllInvoicesAsync();
        return invoices == null || invoices.Count == 0 ? Results.NotFound("No invoices were found") : Results.Ok(invoices);
    }
    public async Task<IResult> GetInvoice(string invoiceId)
    {
        var invoice = await _repo.GetInvoiceByIdAsync(invoiceId);
        return invoice == null ? Results.NotFound($"Invoice with invoice id = {invoiceId} was not found") : Results.Ok(invoice);
    }
    public async Task<IResult> CreateInvoice(Invoice invoice)
    {
        var existing = await _repo.GetInvoiceByIdAsync(invoice.InvoiceId);
        if (existing != null)
            return Results.Conflict($"An invoice with id= {invoice.InvoiceId} already exists.");

        try
        {
            await _repo.AddInvoice(invoice);
            return Results.Ok(invoice);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> UpdateInvoiceDetails(Invoice update, string invoiceId)
    {
        try
        {
            await _repo.UpdateInvoiceDetailsAsync(update, invoiceId);
            Invoice? invoice = await _repo.GetInvoiceByIdAsync(invoiceId);
            return Results.Ok(invoice);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }

    public async Task<IResult> RemoveInvoice(string invoiceId)
    {
        var retrievedInvoice = await _repo.GetInvoiceByIdAsync(invoiceId);
        if (retrievedInvoice == null)
            return Results.NotFound($"Invoice with invoice id = {invoiceId} was not found");


        try
        {
            await _repo.RemoveInvoiceAsync(invoiceId);
            return Results.Ok(retrievedInvoice);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }

    }
}

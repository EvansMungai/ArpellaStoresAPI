using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IInvoiceService
{
    Task<IResult> GetInvoices();
    Task<IResult> GetInvoice(string invoiceId);
    Task<IResult> CreateInvoice(Invoice invoice);
    Task<IResult> UpdateInvoiceDetails(Invoice update, string invoiceId);
    Task<IResult> RemoveInvoice(string invoiceId);
}

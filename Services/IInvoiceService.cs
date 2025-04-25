using ArpellaStores.Models;

namespace ArpellaStores.Services;

public interface IInvoiceService
{
    Task<IResult> GetInvoices();
    Task<IResult> GetInvoice(string id);
    Task<IResult> CreateInvoice(Invoice invoice);
    Task<IResult> UpdateInvoceDetails(Invoice update, string id);
    Task<IResult> RemoveInvoice(string invoiceId);
}

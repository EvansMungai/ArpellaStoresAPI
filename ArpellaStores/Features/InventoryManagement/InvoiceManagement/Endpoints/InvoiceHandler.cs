using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.InventoryManagement.Services;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class InvoiceHandler : IHandler
{
    public static string RouteName => "Invoice Managment";
    private readonly IInvoiceService _invoiceService;
    public InvoiceHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public Task<IResult> GetInvoices() => _invoiceService.GetInvoices();
    public Task<IResult> GetInvoice(string invoiceId) => _invoiceService.GetInvoice(invoiceId);
    public Task<IResult> CreateInvoice(Invoice invoice) => _invoiceService.CreateInvoice(invoice);
    public Task<IResult> UpdateInvoiceDetails(Invoice update, string invoiceId) => _invoiceService.UpdateInvoiceDetails(update, invoiceId);
    public Task<IResult> RemoveInvoice(string invoiceId) => _invoiceService.RemoveInvoice(invoiceId);
}

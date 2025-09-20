using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.OrderManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.PaymentManagement.Services;

public class PaymentResultHelper : IPaymentResultHelper
{
    private readonly ArpellaContext _context;
    public PaymentResultHelper(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetPaymentStatusAsync(string orderid)
    {
        var payment = await _context.Payments.Select(p => new{ p.PaymentId, p.Orderid, p.TransactionId, p.Status }).AsNoTracking().SingleOrDefaultAsync(p => p.Orderid == orderid);
        return Results.Ok(payment);
    }
}

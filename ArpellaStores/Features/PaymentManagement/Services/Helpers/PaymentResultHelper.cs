using ArpellaStores.Data.Infrastructure;
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
        var payment = await _context.Payments.Select(p => new { p.PaymentId, p.Orderid, p.TransactionId, p.Status }).AsNoTracking().SingleOrDefaultAsync(p => p.Orderid == orderid);
        if (payment == null)
        {
            return Results.NotFound($"Order with order id = {orderid} does not exist.");
        }
        return Results.Ok(payment);
    }
}

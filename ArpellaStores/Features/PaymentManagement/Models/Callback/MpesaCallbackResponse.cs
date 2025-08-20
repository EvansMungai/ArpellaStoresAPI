namespace ArpellaStores.Features.PaymentManagement.Models;

public class MpesaCallbackResponse
{
    public string Message { get; set; }
    public string OrderId { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string PhoneNumber { get; set; }

}

namespace ArpellaStores.Features.PaymentManagement.Models;

public class StkCallback
{
    public string MerchantRequestID { get; set; }
    public string CheckoutRequestID { get; set; }
    public int ResultCode { get; set; }
    public string ResultDesc { get; set; }
    public CallbackMetadata CallbackMetadata { get; set; }
}
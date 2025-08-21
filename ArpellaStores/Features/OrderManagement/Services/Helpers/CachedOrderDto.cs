namespace ArpellaStores.Features.OrderManagement.Services;

public class CachedOrderDto
{
    public string Orderid { get; set; }
    public string UserId { get; set; }
    public string PhoneNumber { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string BuyerPin { get; set; }
    public List<CachedOrderItemDto> Orderitems { get; set; }


}

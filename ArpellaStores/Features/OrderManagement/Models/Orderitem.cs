using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Models;

public partial class Orderitem
{
    public string? OrderId { get; set; }
    public int ProductId { get; set; }
    public string? PriceType { get; set; }
    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}

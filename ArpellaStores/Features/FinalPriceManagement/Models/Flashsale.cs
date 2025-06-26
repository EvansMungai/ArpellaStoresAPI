using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.FinalPriceManagement.Models;

public partial class Flashsale
{
    public int FlashSaleId { get; set; }

    public int? ProductId { get; set; }

    public decimal DiscountValue { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? IsActive { get; set; }

    public virtual Product? Product { get; set; }
}

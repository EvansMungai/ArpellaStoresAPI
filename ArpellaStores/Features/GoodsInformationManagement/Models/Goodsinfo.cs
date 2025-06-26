namespace ArpellaStores.Features.GoodsInformationManagement.Models;

public partial class Goodsinfo
{
    public string ItemCode { get; set; } = null!;

    public string? ItemDescription { get; set; }

    public string? UnitMeasure { get; set; }

    public decimal? TaxRate { get; set; }

    public int Id { get; set; }
}

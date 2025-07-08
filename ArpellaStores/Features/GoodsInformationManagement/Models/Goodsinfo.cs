using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.GoodsInformationManagement.Models;

public partial class Goodsinfo
{
    [Required(ErrorMessage = "Item code is required.")]
    [StringLength(30, ErrorMessage = "Item code must be at most 30 characters.")]
    public string ItemCode { get; set; } = null!;

    [StringLength(30, ErrorMessage = "Item Description must be at most 30 characters.")]
    public string? ItemDescription { get; set; }

    [StringLength(30, ErrorMessage = "Unit of measure must be at most ")]
    public string? UnitMeasure { get; set; }

    [Required(ErrorMessage = "Tax Rate is required.")]
    public decimal? TaxRate { get; set; }

    public int Id { get; set; }

    [Required(ErrorMessage = "Product Id is required.")]
    [StringLength(30, ErrorMessage = "Product Id must be at most 30 characters.")]
    public string? ProductId { get; set; }
}

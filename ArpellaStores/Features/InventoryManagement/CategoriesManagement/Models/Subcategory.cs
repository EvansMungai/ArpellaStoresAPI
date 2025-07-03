using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Subcategory
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Subcategory name is required.")]
    [StringLength(50, ErrorMessage = "Subcategory name must be at most 50 characters")]
    public string? SubcategoryName { get; set; }

    [Required(ErrorMessage = "Category Id is required.")]
    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

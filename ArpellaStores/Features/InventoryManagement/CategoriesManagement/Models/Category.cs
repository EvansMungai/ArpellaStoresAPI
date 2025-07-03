using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Category
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Category Name is required")]
    [StringLength(50, ErrorMessage = "Category name must be at most 50 characters")]
    public string? CategoryName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}

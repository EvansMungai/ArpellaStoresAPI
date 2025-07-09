using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.SettingManagement.Models;

public partial class Setting
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Setting Name is required.")]
    [StringLength(30, ErrorMessage = "Setting Name must be at most 30 characters.")]
    public string? SettingName { get; set; }

    [Required(ErrorMessage = "Setting Value is required.")]
    [StringLength(50, ErrorMessage = "Setting Value must be at most 50 characters.")]
    public string? SettingValue { get; set; }
}

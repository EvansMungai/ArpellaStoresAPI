using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.Authentication.Models;

public class ChangePasswordModel
{
    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}

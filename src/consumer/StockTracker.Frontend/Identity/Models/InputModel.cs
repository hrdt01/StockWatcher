using System.ComponentModel.DataAnnotations;

namespace StockTracker.Frontend.Identity.Models;

public sealed class InputModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;

namespace UKG.HCM.UI.Models;

public class LoginRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseModel
{
    public string? Token { get; set; }
}

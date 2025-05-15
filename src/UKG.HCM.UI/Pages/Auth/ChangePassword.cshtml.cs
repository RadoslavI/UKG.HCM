using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Pages.Auth;

[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly IAuthService _authService;

    public ChangePasswordModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public ChangePasswordViewModel PasswordRequest { get; set; } = new();

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // Just render the form
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _authService.ChangePasswordAsync(
            PasswordRequest.CurrentPassword, 
            PasswordRequest.NewPassword);

        if (result.success)
        {
            SuccessMessage = result.message;
            PasswordRequest = new ChangePasswordViewModel(); // Clear the form
        }
        else
        {
            ErrorMessage = result.message;
        }

        return Page();
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UKG.HCM.UI.JWT;
using UKG.HCM.UI.Models;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly JwtTokenStore _tokenStore;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IAuthService authService, JwtTokenStore tokenStore, ILogger<LoginModel> logger)
    {
        _authService = authService;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    [BindProperty]
    public LoginRequestModel LoginRequest { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // If we're already authenticated, redirect to home
        if (User.Identity?.IsAuthenticated == true)
        {
            Response.Redirect("/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try 
        {
            var (success, message) = await _authService.LoginAsync(LoginRequest.Email, LoginRequest.Password);
            if (!success)
            {
                ErrorMessage = message;
                return Page();
            }

            _logger.LogInformation("User logged in successfully: {Email}", LoginRequest.Email);
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", LoginRequest.Email);
            ErrorMessage = "An error occurred during login.";
            return Page();
        }
    }
}
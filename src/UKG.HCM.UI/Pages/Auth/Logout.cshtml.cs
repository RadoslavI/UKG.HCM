using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UKG.HCM.UI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace UKG.HCM.UI.Pages.Auth;

[Authorize]
public class LogoutModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(IAuthService authService, ILogger<LogoutModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _authService.LogoutAsync();
        return RedirectToPage("/Index");
    }
}
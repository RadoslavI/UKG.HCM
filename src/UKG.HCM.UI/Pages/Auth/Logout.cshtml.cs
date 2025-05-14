using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UKG.HCM.UI.Pages.Auth;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // Remove token from session (if stored there)
        HttpContext.Session.Remove("JWT");

        // Optional: clear all session
        // HttpContext.Session.Clear();

        return RedirectToPage("/Auth/Login");
    }
}
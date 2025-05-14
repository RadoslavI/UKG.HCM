using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UKG.HCM.UI.JWT;

namespace UKG.HCM.UI.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JwtTokenStore _tokenStore;

    public LoginModel(IHttpClientFactory httpClientFactory, JwtTokenStore tokenStore)
    {
        _httpClientFactory = httpClientFactory;
        _tokenStore = tokenStore;
    }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _httpClientFactory.CreateClient("AuthApi");

        var requestBody = JsonSerializer.Serialize(new
        {
            Username,
            Password
        });

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/Auth/login", content);
        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Invalid login attempt.";
            return Page();
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (result != null)
        {
            _tokenStore.Token = result.Token;
            return RedirectToPage("/People/Index");
        }

        ErrorMessage = "Something went wrong.";
        return Page();
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
}
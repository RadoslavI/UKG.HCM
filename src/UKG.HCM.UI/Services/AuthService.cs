using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using UKG.HCM.UI.JWT;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;
    private readonly JwtTokenStore _tokenStore;
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthService(
        HttpClient httpClient, 
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger,
        IConfiguration configuration,
        JwtTokenStore tokenStore)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _configuration = configuration;
        _tokenStore = tokenStore;
    }

    private string GetEndpoint(string name)
    {
        var baseUrl = _configuration["ApiEndpoints:AuthenticationApi:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
            throw new InvalidOperationException($"BaseUrl for AuthAPI not found in configuration");
        
        var endpoint = _configuration[$"ApiEndpoints:AuthenticationApi:Endpoints:{name}"];
        if (string.IsNullOrEmpty(endpoint))
            throw new InvalidOperationException($"Endpoint '{name}' not found in configuration");
        
        return new Uri(new Uri(baseUrl), endpoint).ToString();
    }

    public async Task<(bool success, string message)> LoginAsync(string email, string password)
    {
        try
        {
            var loginEndpoint = GetEndpoint("Login");
            
            var loginRequest = new Models.LoginRequestModel
            {
                Email = email,
                Password = password
            };
    
            var response = await _httpClient.PostAsJsonAsync(loginEndpoint, loginRequest);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Login failed for email {Email}. Status code: {StatusCode}", 
                    email, response.StatusCode);
                return (false, $"Login failed: {response.ReasonPhrase}");
            }
    
            var responseContent = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<Models.LoginResponseModel>(responseContent, _jsonOptions);

            if (loginResponse?.Token == null)
            {
                _logger.LogWarning("Login response didn't contain a token for {Email}", email);
                return (false, "Unable to get token");
            }

            // Store token in both the cookie authentication and token store
            _tokenStore.Token = loginResponse.Token;
            await SignInWithTokenAsync(loginResponse.Token);
            
            _logger.LogInformation("User {Email} logged in successfully", email);
            return (true, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", email);
            return (false, "An error occurred during login");
        }
    }
    
    public async Task<(bool success, string message)> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            SetAuthorizationHeader();
            
            var changePasswordEndpoint = GetEndpoint("ChangePassword");
        
            var request = new
            {
                Email = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email),
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var response = await _httpClient.PostAsJsonAsync(changePasswordEndpoint, request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Change password failed. Status code: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                return (false, $"Change password failed: {errorContent}, Error: {response.ReasonPhrase}");
            }

            _logger.LogInformation("Password changed successfully");
            return (true, "Password changed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return (false, "An error occurred while changing password");
        }
    }

    public string? GetToken()
    {
        return _tokenStore.Token;
    }

    public void Logout()
    {
        // Call the async method without awaiting to satisfy the synchronous interface method
        // This is not ideal, but it's a common pattern when adapting async methods to sync interfaces
        LogoutAsync().ConfigureAwait(false);
    }

    public async Task LogoutAsync()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _tokenStore.Token = null;
            _logger.LogInformation("User {Email} logged out", email);
        }
    }
    
    public void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_tokenStore.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _tokenStore.Token);
        }
    }

    private async Task SignInWithTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var claims = new List<Claim>
        {
            new Claim("jwt", token)
        };

        // Add claims from the token
        claims.AddRange(jwtToken.Claims);

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        if (_httpContextAccessor.HttpContext != null)
        {
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });
        }
    }
}

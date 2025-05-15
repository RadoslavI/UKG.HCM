using System.Text.Json;
using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;

namespace UKG.HCM.PeopleManagementApi.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(HttpClient httpClient, ILogger<AuthService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> CreateUserAsync(CreateUserDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_configuration["AuthenticationApi:RegisterEndpoint"], dto);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to create user. Status: {StatusCode}", response.StatusCode);
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while creating user");
            return false;
        }
    }
    
    public async Task<bool> DeleteUserAsync(string email)
    {
        try
        {
            var url = $"{_configuration["AuthenticationApi:DeleteEndpoint"]}?email={Uri.EscapeDataString(email)}";
            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to delete user. Status: {StatusCode}", response.StatusCode);
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while deleting user");
            return false;
        }
    }
}
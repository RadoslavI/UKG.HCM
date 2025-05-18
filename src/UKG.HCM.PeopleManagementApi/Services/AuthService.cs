using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;
using UKG.HCM.Shared.Utilities;

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

    public async Task<OperationResult> CreateUserAsync(UserDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_configuration["AuthenticationApi:RegisterEndpoint"], dto);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Failed to create user. Status: {response.StatusCode}");
                return OperationResult.FailureResult($"Failed to create user. Status: {response.StatusCode}");
            }
            
            return OperationResult.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while creating user");
            return OperationResult.FailureResult(ex.Message);
        }
    }
    
    public async Task<OperationResult> UpdateUserAsync(UserDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_configuration["AuthenticationApi:UpdateEndpoint"], dto);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Failed to update user. Status: {response.StatusCode}");
                return OperationResult.FailureResult($"Failed to update user. Status: {response.StatusCode}");
            }
            
            return OperationResult.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while updating user");
            return OperationResult.FailureResult(ex.Message);;
        }
    }
    
    public async Task<OperationResult> DeleteUserAsync(string email)
    {
        try
        {
            var url = $"{_configuration["AuthenticationApi:DeleteEndpoint"]}/{Uri.EscapeDataString(email)}";
            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Failed to delete user. Status: {response.StatusCode}");
                return OperationResult.FailureResult($"Failed to delete user. Status: {response.StatusCode}");
            }
            
            return OperationResult.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while deleting user");
            return OperationResult.FailureResult(ex.Message);
        }
    }
}
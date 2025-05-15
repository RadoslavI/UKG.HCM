using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UKG.HCM.UI.JWT;
using UKG.HCM.UI.Models;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Services;

public class PeopleService : IPeopleService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PeopleService> _logger;
    private readonly JwtTokenStore _tokenStore;
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PeopleService(
        HttpClient httpClient, 
        IConfiguration configuration, 
        ILogger<PeopleService> logger,
        JwtTokenStore tokenStore)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _tokenStore = tokenStore;
    }

    private string GetEndpoint(string name, params object[] args)
    {
        var baseUrl = _configuration["ApiEndpoints:PeopleManagementApi:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
            throw new InvalidOperationException($"BaseUrl for PeopleManagementAPI not found in configuration");
        
        var endpoint = _configuration[$"ApiEndpoints:PeopleManagementApi:Endpoints:{name}"];
        if (string.IsNullOrEmpty(endpoint))
            throw new InvalidOperationException($"Endpoint '{name}' not found in configuration");
        
        // Replace placeholders in the endpoint URL
        for (int i = 0; i < args.Length; i++)
        {
            endpoint = endpoint.Replace($"{{{i}}}", args[i].ToString());
        }
        
        return new Uri(new Uri(baseUrl), endpoint).ToString();
    }

    private void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_tokenStore.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _tokenStore.Token);
        }
    }
    
    public async Task<IEnumerable<PersonViewModel>> GetPeopleAsync()
    {
        try
        {
            SetAuthorizationHeader();
            var endpoint = GetEndpoint("GetPeople");
            
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var people = JsonSerializer.Deserialize<IEnumerable<PersonViewModel>>(content, _jsonOptions);
            
            return people ?? Enumerable.Empty<PersonViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting people");
            return Enumerable.Empty<PersonViewModel>();
        }
    }

    public async Task<PersonViewModel?> GetPersonByIdAsync(Guid id)
    {
        try
        {
            SetAuthorizationHeader();
            var endpoint = GetEndpoint("GetPerson", id);
            
            var response = await _httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
                return null;
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PersonViewModel>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting person with id {Id}", id);
            return null;
        }
    }

    public async Task<Guid> CreatePersonAsync(CreatePersonViewModel person)
    {
        try
        {
            SetAuthorizationHeader();
            var endpoint = GetEndpoint("CreatePerson");
            
            // Use PostAsJsonAsync which properly handles JSON serialization
            var response = await _httpClient.PostAsJsonAsync(endpoint, person);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Create person response: {Content}", responseContent);
            
            // Handle potential quotes in response
            if (Guid.TryParse(responseContent.Trim('"'), out var id))
            {
                return id;
            }
            
            // If direct parsing fails, try JsonDocument
            try
            {
                using var document = JsonDocument.Parse(responseContent);
                return document.RootElement.GetGuid();
            }
            catch
            {
                throw new FormatException($"Unable to parse response as Guid: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person {FirstName} {LastName}", 
                person.FirstName, person.LastName);
            throw;
        }
    }

    public async Task<(bool Success, string ErrorMessage)> UpdatePersonAsync(Guid id, UpdatePersonViewModel person)
    {
        try
        {
            SetAuthorizationHeader();
            var endpoint = GetEndpoint("UpdatePerson", id);
            
            // Log what we're sending to help debug issues
            _logger.LogDebug("Updating person {Id} with data: {@PersonData}", id, person);
            
            // Use PutAsJsonAsync which properly handles JSON serialization
            var response = await _httpClient.PutAsJsonAsync(endpoint, person);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Update person failed with status code {StatusCode}: {ErrorContent}", 
                    response.StatusCode, errorContent);
                
                // Return the error details
                return (false, $"API Error: {errorContent}");
            }
            
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person with id {Id}", id);
            return (false, $"Exception: {ex.Message}");
        }
    }

    public async Task<bool> DeletePersonAsync(Guid id)
    {
        try
        {
            SetAuthorizationHeader();
            var endpoint = GetEndpoint("DeletePerson", id);
            
            var response = await _httpClient.DeleteAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Delete person failed with status code {StatusCode}: {ErrorContent}", 
                    response.StatusCode, errorContent);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting person with id {Id}", id);
            return false;
        }
    }
}

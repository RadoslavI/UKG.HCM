namespace UKG.HCM.Shared.Configuration;

/// <summary>
/// Configuration for JWT token generation and validation
/// </summary>
public class JwtConfig
{
    /// <summary>
    /// Secret key used for signing JWT tokens
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Issuer of the JWT token (typically the auth service)
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    
    /// <summary>
    /// Intended audience of the JWT token
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Token expiration time in minutes
    /// </summary>
    public int ExpiryInMinutes { get; set; } = 60;
}

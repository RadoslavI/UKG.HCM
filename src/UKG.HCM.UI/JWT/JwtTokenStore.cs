namespace UKG.HCM.UI.JWT;

/// <summary>
/// Simple service to store JWT token in memory during application lifetime
/// </summary>
public class JwtTokenStore
{
    private string? _token;
    
    /// <summary>
    /// Gets or sets the JWT token
    /// </summary>
    public string? Token
    {
        get => _token;
        set => _token = value;
    }
}
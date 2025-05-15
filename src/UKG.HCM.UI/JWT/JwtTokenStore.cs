namespace UKG.HCM.UI.JWT;

/// <summary>
/// A simple service to store the JWT token for the current user's session
/// </summary>
public class JwtTokenStore
{
    /// <summary>
    /// The current JWT token, if any
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// Check if a token is available
    /// </summary>
    public bool HasToken => !string.IsNullOrEmpty(Token);
}
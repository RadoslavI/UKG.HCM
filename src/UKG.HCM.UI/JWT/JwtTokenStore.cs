namespace UKG.HCM.UI.JWT;

/// <summary>
/// Simple service to store JWT token in memory during application lifetime
/// </summary>
public class JwtTokenStore
{
    private string? _token;
    public string? Token
    {
        get => _token;
        set => _token = value;
    }
}
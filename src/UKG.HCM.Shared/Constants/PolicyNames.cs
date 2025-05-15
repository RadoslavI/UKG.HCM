namespace UKG.HCM.Shared.Constants;

/// <summary>
/// Authorization policy names used across the application
/// </summary>
public static class PolicyNames
{
    /// <summary>
    /// Policy for HR administrators only
    /// </summary>
    public const string RequireHRAdmin = "RequireHRAdmin";
    
    /// <summary>
    /// Policy for managers and above
    /// </summary>
    public const string RequireManagerOrAbove = "RequireManagerOrAbove";
    
    /// <summary>
    /// Policy for any authenticated user
    /// </summary>
    public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";
}

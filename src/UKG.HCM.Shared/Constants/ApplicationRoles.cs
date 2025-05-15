namespace UKG.HCM.Shared.Constants;

/// <summary>
/// Application roles for authorization
/// </summary>
public static class ApplicationRoles
{
    /// <summary>
    /// HR Administrator role with full system access
    /// </summary>
    public const string HRAdmin = "HRAdmin";
    
    /// <summary>
    /// Department Manager role for team management
    /// </summary>
    public const string Manager = "Manager";
    
    /// <summary>
    /// Regular employee role with limited access
    /// </summary>
    public const string Employee = "Employee";
}

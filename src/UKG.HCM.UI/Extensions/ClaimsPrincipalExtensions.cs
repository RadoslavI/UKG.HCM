using System.Security.Claims;

namespace UKG.HCM.UI.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Checks if the current user is in the specified role
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal</param>
    /// <param name="role">The role to check</param>
    /// <returns>True if the user is in the role, false otherwise</returns>
    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        if (principal is null)
            return false;
            
        return principal.HasClaim(ClaimTypes.Role, role);
    }
    
    /// <summary>
    /// Checks if the current user is an HR Admin
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal</param>
    /// <returns>True if the user is an HR Admin, false otherwise</returns>
    public static bool IsHRAdmin(this ClaimsPrincipal principal)
    {
        return principal.IsInRole("HRAdmin");
    }
    
    /// <summary>
    /// Checks if the current user is a Manager
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal</param>
    /// <returns>True if the user is a Manager, false otherwise</returns>
    public static bool IsManager(this ClaimsPrincipal principal)
    {
        return principal.IsInRole("Manager");
    }
    
    /// <summary>
    /// Checks if the current user is an Employee
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal</param>
    /// <returns>True if the user is an Employee, false otherwise</returns>
    public static bool IsEmployee(this ClaimsPrincipal principal)
    {
        return principal.IsInRole("Employee");
    }
    
    /// <summary>
    /// Checks if the current user is at least a Manager (Manager or HRAdmin)
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal</param>
    /// <returns>True if the user is at least a Manager, false otherwise</returns>
    public static bool IsManagerOrAbove(this ClaimsPrincipal principal)
    {
        return principal.IsInRole("Manager") || principal.IsInRole("HRAdmin");
    }
}

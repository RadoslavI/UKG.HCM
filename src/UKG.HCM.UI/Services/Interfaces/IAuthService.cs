namespace UKG.HCM.UI.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="password">Password</param>
    /// <returns>Success flag and message</returns>
    Task<(bool success, string message)> LoginAsync(string email, string password);
    
    /// <summary>
    /// Logs out the current user
    /// </summary>
    Task LogoutAsync();
}

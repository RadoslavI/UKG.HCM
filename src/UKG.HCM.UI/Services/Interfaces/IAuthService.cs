namespace UKG.HCM.UI.Services.Interfaces;

public interface IAuthService
{
    Task<(bool success, string message)> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<(bool success, string message)> ChangePasswordAsync(string currentPassword, string newPassword);
    string? GetToken();
    void SetAuthorizationHeader();
    void Logout();
}

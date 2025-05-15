namespace UKG.HCM.AuthenticationApi.Data.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    
    public string PasswordHash { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Role { get; set; } = null!;
}
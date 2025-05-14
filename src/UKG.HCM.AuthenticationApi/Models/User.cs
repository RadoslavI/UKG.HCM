namespace UKG.HCM.AuthenticationApi.Models;

public class User
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!; // NOTE: In real apps, hash this!
    public string Role { get; set; } = null!;
}
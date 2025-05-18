namespace UKG.HCM.AuthenticationApi.DTOs.CreateUser;

public class IncomingCreateOrUpdateUserDto
{
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string Role { get; set; }
    public string? Password { get; set; }
}
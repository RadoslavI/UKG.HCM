using System.Security.Cryptography;
using System.Text;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Models;
using UKG.HCM.AuthenticationApi.Services.Interfaces;

namespace UKG.HCM.AuthenticationApi.Services;

public class UserService : IUserService
{
    private readonly List<User> _users =
    [
        new() { Id = Guid.NewGuid(), FullName = "employee1", PasswordHash = "1234", Role = "Employee" },
        new() { Id = Guid.NewGuid(), FullName = "manager1", PasswordHash = "1234", Role = "Manager" },
        new() { Id = Guid.NewGuid(), FullName = "admin1", PasswordHash = "1234", Role = "HRAdmin" }
    ];

    public User? ValidateUser(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            u.FullName.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.PasswordHash == password);
    }

    public Task<bool> CreateUserAsync(IncomingCreateUserDto dto)
    {
        if (_users.Any(u => u.Email == dto.Email))
            return Task.FromResult(false); // Already exists

        var password = dto.Password ?? GenerateRandomPassword();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FullName = dto.FullName,
            Role = dto.Role,
            PasswordHash = HashPassword(password)
        };

        _users.Add(user);

        // You could email the password here if needed
        return Task.FromResult(true);
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }

    private static string GenerateRandomPassword()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }
}
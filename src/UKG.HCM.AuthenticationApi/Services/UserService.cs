using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using UKG.HCM.AuthenticationApi.Data;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Models;
using UKG.HCM.AuthenticationApi.Services.Interfaces;
using UKG.HCM.AuthenticationApi.Utilities;

namespace UKG.HCM.AuthenticationApi.Services;

public class UserService : IUserService
{
    private readonly AuthContext _context;
    
    public UserService(AuthContext context)
    {
        _context = context;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var passwordHash = PasswordHasher.HashPassword(password);
        
        // Allow login with either fullname or email
        return await _context.Users.FirstOrDefaultAsync(u =>
            (u.FullName.Equals(username, StringComparison.OrdinalIgnoreCase) ||
             u.Email.Equals(username, StringComparison.OrdinalIgnoreCase)) &&
            u.PasswordHash == passwordHash);
    }

    public async Task<bool> CreateUserAsync(IncomingCreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return false; // Already exists

        var password = dto.Password ?? PasswordHasher.GenerateRandomPassword();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FullName = dto.FullName,
            Role = dto.Role,
            PasswordHash = PasswordHasher.HashPassword(password)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        // You could email the password here if needed
        return true;
    }
    
    public IEnumerable<Claim> GetUserClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        
        return claims;
    }
}
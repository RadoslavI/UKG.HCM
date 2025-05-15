using Microsoft.EntityFrameworkCore;
using UKG.HCM.AuthenticationApi.Data;
using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Services.Interfaces;
using UKG.HCM.AuthenticationApi.Utilities;

namespace UKG.HCM.AuthenticationApi.Services;

public class UserService : IUserService
{
    private readonly AuthContext _context;
    private readonly ILogger _logger;
    
    public UserService(AuthContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var passwordHash = PasswordHasher.HashPassword(password);
        
        return await _context.Users.FirstOrDefaultAsync(u =>
            u.Email.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.PasswordHash == passwordHash);
    }

    public async Task<bool> CreateUserAsync(IncomingCreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return false;

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
        
        // Email to the user with the temp password to be sent here instead if in production env
        _logger.LogInformation("User {0} was created with password: {1}", user.Email, password);
        return true;
    }
    
    public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
    {
        var user = await ValidateUserAsync(email, currentPassword);
        if (user is null)
        {
            _logger.LogWarning("User {0} not found or password is incorrect", email);
            return false;
        }
    
        user.PasswordHash = PasswordHasher.HashPassword(newPassword);
        await _context.SaveChangesAsync();
    
        _logger.LogInformation("Password for user {0} changed successfully", email);
        
        return true;
    }
    
    public async Task<bool> DeleteUserAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {0} deleted successfully", email);
        return true;
    }
}
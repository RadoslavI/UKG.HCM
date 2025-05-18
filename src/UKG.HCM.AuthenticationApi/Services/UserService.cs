using Microsoft.EntityFrameworkCore;
using UKG.HCM.AuthenticationApi.Data;
using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Services.Interfaces;
using UKG.HCM.Shared.Utilities;

namespace UKG.HCM.AuthenticationApi.Services;

public class UserService : IUserService
{
    private readonly AuthContext _context;
    private readonly ILogger<UserService> _logger;
    
    public UserService(AuthContext context, ILogger<UserService> logger)
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

    public async Task<OperationResult> CreateUserAsync(IncomingCreateOrUpdateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return OperationResult.FailureResult("User already exists");    

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
        return OperationResult.SuccessResult();    
    }

    public async Task<OperationResult> UpdateUserAsync(IncomingCreateOrUpdateUserDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null)
        {
            _logger.LogWarning("User {0} not found", dto.Email);
            return OperationResult.FailureResult($"User {dto.Email} not found");    
        } 
        
        user.FullName = dto.FullName;
        user.Role = dto.Role;
        user.Email = dto.Email;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {0} was updated successfully", user.Email);
        return OperationResult.SuccessResult();
    }

    public async Task<OperationResult> ChangePasswordAsync(string email, string currentPassword, string newPassword)
    {
        var user = await ValidateUserAsync(email, currentPassword);
        if (user is null)
        {
            _logger.LogWarning("User {0} not found or password is incorrect", email);
            return OperationResult.FailureResult($"User {email} not found or password is incorrect");
        }
    
        user.PasswordHash = PasswordHasher.HashPassword(newPassword);
        await _context.SaveChangesAsync();
    
        _logger.LogInformation("Password for user {0} changed successfully", email);
        return OperationResult.SuccessResult();
    }
    
    public async Task<OperationResult> DeleteUserAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return OperationResult.FailureResult($"User {email} not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {0} deleted successfully", email);
        return OperationResult.SuccessResult();
    }
}
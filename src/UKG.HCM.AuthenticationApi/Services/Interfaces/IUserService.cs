using System.Security.Claims;
using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;

namespace UKG.HCM.AuthenticationApi.Services.Interfaces;

public interface IUserService
{
    Task<User?> ValidateUserAsync(string username, string password);
    Task<bool> CreateUserAsync(IncomingCreateUserDto dto);
    IEnumerable<Claim> GetUserClaims(User user);
    Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
}
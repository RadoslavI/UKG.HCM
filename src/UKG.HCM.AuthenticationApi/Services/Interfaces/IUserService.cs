using System.Security.Claims;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Models;

namespace UKG.HCM.AuthenticationApi.Services.Interfaces;

public interface IUserService
{
    Task<User?> ValidateUserAsync(string username, string password);
    Task<bool> CreateUserAsync(IncomingCreateUserDto dto);
    IEnumerable<Claim> GetUserClaims(User user);
}
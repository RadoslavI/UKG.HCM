using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.Models;

namespace UKG.HCM.AuthenticationApi.Services.Interfaces;

public interface IUserService
{
    User? ValidateUser(string username, string password);
    Task<bool> CreateUserAsync(IncomingCreateUserDto dto);
}
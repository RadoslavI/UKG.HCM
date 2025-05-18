using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.Shared.Utilities;

namespace UKG.HCM.AuthenticationApi.Services.Interfaces;

public interface IUserService
{
    Task<User?> ValidateUserAsync(string username, string password);
    Task<OperationResult> CreateUserAsync(IncomingCreateOrUpdateUserDto dto);
    Task<OperationResult> UpdateUserAsync(IncomingCreateOrUpdateUserDto dto);
    Task<OperationResult> ChangePasswordAsync(string email, string currentPassword, string newPassword);
    Task<OperationResult> DeleteUserAsync(string email);
}
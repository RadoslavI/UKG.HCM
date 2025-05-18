using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.Shared.Utilities;

namespace UKG.HCM.PeopleManagementApi.Services.Interfaces;

public interface IAuthService
{
    Task<OperationResult> CreateUserAsync(UserDto dto);
    Task<OperationResult> DeleteUserAsync(string email);
    Task<OperationResult> UpdateUserAsync(UserDto dto);
}
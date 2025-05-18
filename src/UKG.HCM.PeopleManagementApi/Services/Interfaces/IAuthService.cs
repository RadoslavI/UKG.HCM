using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;

namespace UKG.HCM.PeopleManagementApi.Services.Interfaces;

public interface IAuthService
{
    Task<bool> CreateUserAsync(UserDto dto);
    Task<bool> DeleteUserAsync(string email);
    Task<bool> UpdateUserAsync(UserDto dto);
}
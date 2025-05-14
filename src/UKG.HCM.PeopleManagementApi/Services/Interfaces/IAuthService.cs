using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;

namespace UKG.HCM.PeopleManagementApi.Services.Interfaces;

public interface IAuthService
{
    Task<bool> CreateUserAsync(CreateUserDto dto);
}
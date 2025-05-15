using UKG.HCM.Shared.Constants;

namespace UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = ApplicationRoles.Employee;
}
using UKG.HCM.PeopleManagementApi.DTOs.Person;
using UKG.HCM.PeopleManagementApi.Models;

namespace UKG.HCM.PeopleManagementApi.Extentions;

public static class RoleTransformations
{
    public static Role FromEnumDTOtoEnum(RoleDTO role)
    {
        return role switch
        {
            RoleDTO.Employee => Role.Employee,
            RoleDTO.Manager => Role.Manager,
            RoleDTO.HRAdmin => Role.HRAdmin,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
    
    public static RoleDTO FromEnumtoEnumDTO(Role role)
    {
        return role switch
        {
            Role.Employee => RoleDTO.Employee,
            Role.Manager => RoleDTO.Manager,
            Role.HRAdmin => RoleDTO.HRAdmin,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}
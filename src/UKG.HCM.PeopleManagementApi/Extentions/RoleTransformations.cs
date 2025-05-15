using UKG.HCM.PeopleManagementApi.Data.Entities;
using UKG.HCM.PeopleManagementApi.DTOs.Person;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.PeopleManagementApi.Extentions;

public static class RoleTransformations
{
    public static string FromEnumToString(Role role)
    {
        return role switch
        {
            Role.Employee => ApplicationRoles.Employee,
            Role.Manager => ApplicationRoles.Manager,
            Role.HRAdmin => ApplicationRoles.HRAdmin,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
    
    public static Role FromStringToEnum(string role)
    {
        return role switch
        {
            ApplicationRoles.Employee => Role.Employee,
            ApplicationRoles.Manager => Role.Manager,
            ApplicationRoles.HRAdmin => Role.HRAdmin,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}
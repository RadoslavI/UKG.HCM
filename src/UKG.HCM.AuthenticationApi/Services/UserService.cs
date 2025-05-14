using UKG.HCM.AuthenticationApi.Models;
using UKG.HCM.AuthenticationApi.Services.Interfaces;

namespace UKG.HCM.AuthenticationApi.Services;

public class UserService : IUserService
{
    private readonly List<User> _users =
    [
        new() { Username = "employee1", Password = "1234", Role = "Employee" },
        new() { Username = "manager1", Password = "1234", Role = "Manager" },
        new() { Username = "admin1", Password = "1234", Role = "HRAdmin" }
    ];

    public User? ValidateUser(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
    }
}
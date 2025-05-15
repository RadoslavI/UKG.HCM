using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.AuthenticationApi.Utilities;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.AuthenticationApi.Data;

public static class DbSeeder
{
    public static void Seed(AuthContext context)
    {
        // Only seed if the database is empty
        if (context.Users.Any()) return;
        
        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Jerry Admin",
                Email = "admin@ukg.com",
                PasswordHash = PasswordHasher.HashPassword("Admin@123"),
                Role = ApplicationRoles.HRAdmin
            },
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Jane Manager",
                Email = "manager@ukg.com",
                PasswordHash = PasswordHasher.HashPassword("Manager@123"),
                Role = ApplicationRoles.Manager
            },
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "John Employee",
                Email = "employee@ukg.com",
                PasswordHash = PasswordHasher.HashPassword("Employee@123"),
                Role = ApplicationRoles.Employee
            }
        };
        
        context.Users.AddRange(users);
        context.SaveChanges();
    }
}

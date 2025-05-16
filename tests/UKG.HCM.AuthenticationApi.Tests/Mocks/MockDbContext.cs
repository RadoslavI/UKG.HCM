using Microsoft.EntityFrameworkCore;
using UKG.HCM.AuthenticationApi.Data;
using UKG.HCM.AuthenticationApi.Data.Entities;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.AuthenticationApi.Tests.Mocks;

public static class MockDbContext
{
    public static AuthContext GetMockDbContext()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"AuthenticationApiTestDb_{Guid.NewGuid()}")
            .Options;

        var context = new AuthContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static async Task<List<User>> SeedUsers(AuthContext context)
    {
        var people = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = "john.doe@example.com",
                Role = ApplicationRoles.HRAdmin,
                PasswordHash = "hashedpassword123",
            },
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Jane Smith",
                Email = "jane.smith@example.com",
                Role = ApplicationRoles.Manager,
                PasswordHash = "hashedpassword456",
            },
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Bob Johnson",
                Email = "bob.johnson@example.com",
                Role = ApplicationRoles.HRAdmin,
                PasswordHash = "hashedpassword789",
            }
        };

        await context.Users.AddRangeAsync(people);
        await context.SaveChangesAsync();

        return people;
    }
}

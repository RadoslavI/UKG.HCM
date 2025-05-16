using Microsoft.EntityFrameworkCore;
using UKG.HCM.PeopleManagementApi.Data;
using UKG.HCM.PeopleManagementApi.Data.Entities;

namespace UKG.HCM.PeopleManagementApi.Tests.Mocks;

public static class MockDbContext
{
    public static PeopleContext GetMockDbContext()
    {
        var options = new DbContextOptionsBuilder<PeopleContext>()
            .UseInMemoryDatabase(databaseName: $"PeopleManagementApiTestDb_{Guid.NewGuid()}")
            .Options;

        var context = new PeopleContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static async Task<List<Person>> SeedPeople(PeopleContext context)
    {
        var people = new List<Person>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = Role.HRAdmin,
                CreatedAt = DateTime.UtcNow,
                HireDate = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Role = Role.Manager,
                CreatedAt = DateTime.UtcNow,
                HireDate = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@example.com",
                Role = Role.HRAdmin,
                CreatedAt = DateTime.UtcNow,
                HireDate = DateTime.UtcNow
            }
        };

        await context.People.AddRangeAsync(people);
        await context.SaveChangesAsync();

        return people;
    }
}

using Microsoft.EntityFrameworkCore;
using UKG.HCM.PeopleManagementApi.Data;
using UKG.HCM.PeopleManagementApi.Data.Entities;

namespace UKG.HCM.PeopleManagementApi.Tests.Integration.Helpers;

public static class DatabaseHelpers
{
    public static async Task<List<Person>> SeedPeople(PeopleContext context)
    {
        // Clear any existing people
        context.People.RemoveRange(await context.People.ToListAsync());
        await context.SaveChangesAsync();
        
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
                Role = Role.Employee,
                CreatedAt = DateTime.UtcNow,
                HireDate = DateTime.UtcNow
            }
        };

        await context.People.AddRangeAsync(people);
        await context.SaveChangesAsync();

        return people;
    }

    public static async Task ClearDatabase(PeopleContext context)
    {
        context.People.RemoveRange(await context.People.ToListAsync());
        await context.SaveChangesAsync();
    }
}

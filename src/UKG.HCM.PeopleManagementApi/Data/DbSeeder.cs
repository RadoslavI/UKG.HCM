using UKG.HCM.PeopleManagementApi.Models;

namespace UKG.HCM.PeopleManagementApi.Data;

public static class DbSeeder
{
    public static void Seed(PeopleContext context)
    {
        if (!context.People.Any())
        {
            context.People.AddRange(
                new Person { FirstName = "John", LastName = "Doe", Email = "john.doe@company.com", Role = "Employee" },
                new Person { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@company.com", Role = "Manager" }
            );
            context.SaveChanges();
        }
    }
}
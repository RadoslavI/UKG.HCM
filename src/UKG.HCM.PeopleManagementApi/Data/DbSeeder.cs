using UKG.HCM.PeopleManagementApi.Data.Entities;

namespace UKG.HCM.PeopleManagementApi.Data;

public static class DbSeeder
{
    public static void Seed(PeopleContext context)
    {
        if (!context.People.Any())
        {
            context.People.AddRange(
                new Person { FirstName = "John", LastName = "Employee", Email = "employee@ukg.com", Role = Role.Employee },
                new Person { FirstName = "Jane", LastName = "Manager", Email = "manager@ukg.com", Role = Role.Manager },
                new Person { FirstName = "Jerry", LastName = "Admin", Email = "admin@ukg.com", Role = Role.HRAdmin }
            );
            context.SaveChanges();
        }
    }
}
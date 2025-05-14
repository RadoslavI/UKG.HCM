using UKG.HCM.PeopleManagementApi.Data.Entities;

namespace UKG.HCM.PeopleManagementApi.Data;

public static class DbSeeder
{
    public static void Seed(PeopleContext context)
    {
        if (!context.People.Any())
        {
            context.People.AddRange(
                new Person { FirstName = "John", LastName = "Doe", Email = "john.doe@company.com", Role = Role.Employee },
                new Person { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@company.com", Role = Role.Manager },
                new Person { FirstName = "Tom", LastName = "Cruise", Email = "tom.cruise@company.com", Role = Role.HRAdmin }
            );
            context.SaveChanges();
        }
    }
}
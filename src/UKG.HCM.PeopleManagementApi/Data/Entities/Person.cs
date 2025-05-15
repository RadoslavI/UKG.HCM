namespace UKG.HCM.PeopleManagementApi.Data.Entities;

public class Person
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Email { get; set; } = null!;

    public Role Role { get; set; } = Role.Employee;
    
    public DateTime HireDate { get; set; } = DateTime.Now;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum Role
{
    HRAdmin,
    Manager,
    Employee
}
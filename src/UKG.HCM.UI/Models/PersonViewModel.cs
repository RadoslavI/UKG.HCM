using System.ComponentModel.DataAnnotations;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.UI.Models;

public class PersonViewModel
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Role { get; set; } = ApplicationRoles.Employee;

    public string FullName => $"{FirstName} {LastName}";
}

public class CreatePersonViewModel
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = ApplicationRoles.Employee;
}

public class UpdatePersonViewModel
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = ApplicationRoles.Employee;
}
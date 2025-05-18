using Microsoft.EntityFrameworkCore;
using UKG.HCM.PeopleManagementApi.Data;
using UKG.HCM.PeopleManagementApi.Data.Entities;
using UKG.HCM.PeopleManagementApi.DTOs.AuthAPI;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Create;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Get;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Update;
using UKG.HCM.PeopleManagementApi.Extentions;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;

namespace UKG.HCM.PeopleManagementApi.Services;

public class PeopleService : IPeopleService
{
    private readonly PeopleContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<PeopleService> _logger;

    public PeopleService(PeopleContext context, IAuthService authService, ILogger<PeopleService> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    public async Task<IEnumerable<OutgoingGetPersonDTO>> GetPeopleAsync()
    {
        var people = await _context.People.ToListAsync();
        
        _logger.LogInformation("People retrieved: {Count}", people.Count);
        return people.Select(p => new OutgoingGetPersonDTO(p.Id, p.FirstName, p.LastName, p.Email, RoleTransformations.FromEnumToString(p.Role), p.HireDate));
    }

    public async Task<OutgoingGetPersonDTO?> GetPersonByIdAsync(Guid id)
    {
        var person = await _context.People.FindAsync(id);
        if (person is null)
        {
            _logger.LogWarning("Person not found: {Id}", id);
            return null;
        }
        return new OutgoingGetPersonDTO(person.Id, person.FirstName, person.LastName, person.Email, RoleTransformations.FromEnumToString(person.Role), person.HireDate);
    }

    public async Task<Guid> CreatePersonAsync(IncomingCreatePersonDTO incoming)
    {
        var person = new Person
        {
            FirstName = incoming.FirstName,
            LastName = incoming.LastName,
            Email = incoming.Email,
            Role = RoleTransformations.FromStringToEnum(incoming.Role),
            HireDate = incoming.HireDate,
            CreatedAt = DateTime.Now
        };
        
        await _context.People.AddAsync(person);

        var userDto = new UserDto
        {
            Email = person.Email,
            FullName = person.FirstName + " " + person.LastName,
            Role = person.Role.ToString()
        };

        var success = await _authService.UpdateUserAsync(userDto);
        if (!success)
        {
            _logger.LogWarning("User creation in Auth API failed for {Email}", person.Email);
        }
        
        _logger.LogInformation("Person updated: {email}", person.Email);
        await _context.SaveChangesAsync();
        return person.Id;
    }

    public async Task<bool> UpdatePersonAsync(Guid id, IncomingUpdatePersonDTO incoming)
    {
        var person = await _context.People.FindAsync(id);
        if (person is null)
        {
            _logger.LogWarning("Update failed: Person with ID {Id} not found", id);
            return false;
        }

        person.FirstName = incoming.FirstName;
        person.LastName = incoming.LastName;
        person.Email = incoming.Email;
        person.Role = RoleTransformations.FromStringToEnum(incoming.Role);
        
        var userDto = new UserDto
        {
            Email = person.Email,
            FullName = person.FirstName + " " + person.LastName,
            Role = person.Role.ToString()
        };

        var success = await _authService.CreateUserAsync(userDto);
        if (!success)
        {
            _context.People.Remove(person);
            _logger.LogWarning("Person created but user creation in Auth API failed for {Email}", person.Email);
        }

        _logger.LogInformation("Person updated: {FirstName} {LastName}", person.FirstName, person.LastName);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePersonAsync(Guid id)
    {
        var person = await _context.People.FindAsync(id);
        if (person is null)
        {
            _logger.LogWarning("Delete failed: Person with ID {Id} not found", id);
            return false;
        }
        
        var success = await _authService.DeleteUserAsync(person.Email);
        if (!success)
        {
            _logger.LogWarning("Failed to delete user in Auth API for {Email}", person.Email);
            return false;
        }
        
        _context.People.Remove(person);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Person deleted: {FirstName} {LastName}", person.FirstName, person.LastName);
        return true;
    }
}
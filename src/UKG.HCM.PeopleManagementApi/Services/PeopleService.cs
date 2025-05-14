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
        return people.Select(p => new OutgoingGetPersonDTO(p.Id, p.FirstName, p.LastName, p.Email, RoleTransformations.FromEnumtoEnumDTO(p.Role)));
    }

    public async Task<OutgoingGetPersonDTO?> GetPersonByIdAsync(Guid id)
    {
        var person = await _context.People.FindAsync(id);
        if (person is null)
        {
            _logger.LogWarning("Person not found: {Id}", id);
            return null;
        }
        return new OutgoingGetPersonDTO(person.Id, person.FirstName, person.LastName, person.Email, RoleTransformations.FromEnumtoEnumDTO(person.Role));
    }

    public async Task<Guid> CreatePersonAsync(IncomingCreatePersonDTO incoming)
    {
        var person = new Person
        {
            FirstName = incoming.FirstName,
            LastName = incoming.LastName,
            Email = incoming.Email,
            Role = RoleTransformations.FromEnumDTOtoEnum(incoming.Role)
        };
        
        await _context.People.AddAsync(person);

        var userDto = new CreateUserDto
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
        
        _logger.LogInformation("Person created: {FirstName} {LastName}", person.FirstName, person.LastName);
        await _context.SaveChangesAsync();
        return person.Id;
    }

    public async Task<bool> UpdatePersonAsync(Guid id, IncomingUpdatePersonDTO incoming)
    {
        var person = await _context.People.FindAsync(id);
        if (person is null)
            return false;

        person.FirstName = incoming.FirstName;
        person.LastName = incoming.LastName;
        person.Email = incoming.Email;
        person.Role = RoleTransformations.FromEnumDTOtoEnum(incoming.Role);

        _logger.LogInformation("Person updated: {FirstName} {LastName}", person.FirstName, person.LastName);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePersonAsync(Guid id)
    {
        var person = await _context.People.FindAsync(id);
        if (person is null)
            return false;

        _context.People.Remove(person);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Person deleted: {FirstName} {LastName}", person.FirstName, person.LastName);
        return true;
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UKG.HCM.PeopleManagementApi.Data;
using UKG.HCM.PeopleManagementApi.Models;

namespace UKG.HCM.PeopleManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly PeopleContext _context;

    public PeopleController(PeopleContext context)
    {
        _context = context;
    }

    // GET: api/people
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Person>>> GetPeople()
    {
        return await _context.People.ToListAsync();
    }

    // GET: api/people/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Person>> GetPerson(Guid id)
    {
        var person = await _context.People.FindAsync(id);
        if (person == null)
            return NotFound();

        return person;
    }

    // POST: api/people
    [HttpPost]
    [Authorize(Roles = "HRAdmin,Manager")]
    public async Task<ActionResult<Person>> CreatePerson(Person person)
    {
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
    }

    // PUT: api/people/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "HRAdmin,Manager")]
    public async Task<IActionResult> UpdatePerson(Guid id, Person updatedPerson)
    {
        if (id != updatedPerson.Id)
            return BadRequest();

        var person = await _context.People.FindAsync(id);
        if (person == null)
            return NotFound();

        person.FirstName = updatedPerson.FirstName;
        person.LastName = updatedPerson.LastName;
        person.Email = updatedPerson.Email;
        person.Role = updatedPerson.Role;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/people/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "HRAdmin")]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        var person = await _context.People.FindAsync(id);
        if (person == null)
            return NotFound();

        _context.People.Remove(person);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UKG.HCM.PeopleManagementApi.Constants;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Create;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Get;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Update;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;

namespace UKG.HCM.PeopleManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController(IPeopleService peopleService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<OutgoingGetPersonDTO>>> GetPeople()
    {
        var people = await peopleService.GetPeopleAsync();
        return Ok(people);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<OutgoingGetPersonDTO>> GetPerson(Guid id)
    {
        var person = await peopleService.GetPersonByIdAsync(id);
        if (person is null)
            return NotFound();

        return Ok(person);
    }

    [HttpPost]
    [Authorize(Roles = $"{ApplicationRoles.HRAdmin}, {ApplicationRoles.Manager}")]
    public async Task<ActionResult<Guid>> CreatePerson(IncomingCreatePersonDTO incoming)
    {
        var createdId = await peopleService.CreatePersonAsync(incoming);
        return CreatedAtAction(nameof(CreatePerson), new { id = createdId });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{ApplicationRoles.HRAdmin}, {ApplicationRoles.Manager}")]
    public async Task<IActionResult> UpdatePerson(Guid id, IncomingUpdatePersonDTO incoming)
    {
        var updated = await peopleService.UpdatePersonAsync(id, incoming);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{ApplicationRoles.HRAdmin}")]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        var deleted = await peopleService.DeletePersonAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

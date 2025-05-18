using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Create;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Get;
using UKG.HCM.PeopleManagementApi.DTOs.Person.Update;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.PeopleManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PolicyNames.RequireAuthenticatedUser)]
public class PeopleController(IPeopleService peopleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutgoingGetPersonDTO>>> GetPeople()
    {
        var people = await peopleService.GetPeopleAsync();
        return Ok(people);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OutgoingGetPersonDTO>> GetPerson(Guid id)
    {
        var person = await peopleService.GetPersonByIdAsync(id);
        if (person is null)
            return NotFound();

        return Ok(person);
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.RequireManagerOrAbove)]
    public async Task<ActionResult<Guid>> CreatePerson(IncomingCreatePersonDTO incoming)
    {
        var result = await peopleService.CreatePersonAsync(incoming);
        return CreatedAtAction(nameof(GetPerson), new { id = result }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyNames.RequireManagerOrAbove)]
    public async Task<IActionResult> UpdatePerson(Guid id, IncomingUpdatePersonDTO incoming)
    {
        var result = await peopleService.UpdatePersonAsync(id, incoming);
        return result.Success ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyNames.RequireHRAdmin)]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        var result = await peopleService.DeletePersonAsync(id);
        return result.Success ? NoContent() : NotFound();
    }
}

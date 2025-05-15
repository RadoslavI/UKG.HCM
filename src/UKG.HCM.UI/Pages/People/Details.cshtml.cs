using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using UKG.HCM.UI.Extensions;
using UKG.HCM.UI.Models;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Pages.People;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IPeopleService peopleService, ILogger<DetailsModel> logger)
    {
        _peopleService = peopleService;
        _logger = logger;
    }

    public PersonViewModel? Person { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        try
        {
            Person = await _peopleService.GetPersonByIdAsync(id);
            
            if (Person == null)
            {
                return NotFound();
            }

            // Check authorization - only admins, managers, or the person themselves can view details
            if (!User.IsInRole("HRAdmin") && !User.IsInRole("Manager"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail) || 
                    !Person.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving person with ID {PersonId}", id);
            StatusMessage = "Error: Failed to retrieve person details.";
            return RedirectToPage("./Index");
        }
    }
}

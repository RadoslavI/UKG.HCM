using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UKG.HCM.Shared.Constants;
using UKG.HCM.UI.Models;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Pages.People;

[Authorize(Roles = ApplicationRoles.HRAdmin)]
public class DeleteModel : PageModel
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IPeopleService peopleService, ILogger<DeleteModel> logger)
    {
        _peopleService = peopleService;
        _logger = logger;
    }

    [BindProperty]
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

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving person with ID {PersonId} for deletion confirmation", id);
            StatusMessage = "Error: Failed to retrieve person details for deletion.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        try
        {
            var success = await _peopleService.DeletePersonAsync(id);
            if (success)
            {
                StatusMessage = "Person was deleted successfully.";
            }
            else
            {
                StatusMessage = "Error: Person not found or could not be deleted.";
            }

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting person with ID {PersonId}", id);
            StatusMessage = "Error: Failed to delete person.";
            
            // Try to refetch the person for the delete confirmation page
            try
            {
                Person = await _peopleService.GetPersonByIdAsync(id);
            }
            catch
            {
                // Do nothing if refetch fails
            }
            
            return Page();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using UKG.HCM.UI.Extensions;
using UKG.HCM.Shared.Constants;
using UKG.HCM.UI.Models;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Pages.People;

[Authorize(Roles = $"{ApplicationRoles.HRAdmin},{ApplicationRoles.Manager}")]
public class EditModel : PageModel
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IPeopleService peopleService, ILogger<EditModel> logger)
    {
        _peopleService = peopleService;
        _logger = logger;
    }

    [BindProperty]
    public UpdatePersonViewModel Person { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        try
        {
            var person = await _peopleService.GetPersonByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            // Map to the update model
            Person = new UpdatePersonViewModel
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Email = person.Email,
                Role = person.Role
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving person with ID {PersonId} for editing", id);
            StatusMessage = "Error: Failed to retrieve person details for editing.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Log role value to help debug role validation errors
            _logger.LogDebug("Submitting update with Role value: '{Role}'", Person.Role);
            
            var (success, errorMessage) = await _peopleService.UpdatePersonAsync(id, Person);
            if (success)
            {
                StatusMessage = "Person was updated successfully.";
                return RedirectToPage("./Details", new { id });
            }
            else
            {
                // Show more specific error message
                if (errorMessage.Contains("Role is invalid"))
                {
                    ModelState.AddModelError("Person.Role", "The selected role is not valid");
                    StatusMessage = "Error: The role you selected is not valid. Please choose a valid role.";
                    return Page();
                }
                
                StatusMessage = $"Error: Person could not be updated. {errorMessage}";
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person with ID {PersonId}", id);
            StatusMessage = "Error: Failed to update person.";
            return Page();
        }
    }
}

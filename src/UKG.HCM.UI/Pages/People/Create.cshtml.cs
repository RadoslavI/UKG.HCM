using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UKG.HCM.Shared.Constants;
using UKG.HCM.UI.Models;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Pages.People;

[Authorize(Roles = $"{ApplicationRoles.HRAdmin},{ApplicationRoles.Manager}")]
public class CreateModel : PageModel
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IPeopleService peopleService, ILogger<CreateModel> logger)
    {
        _peopleService = peopleService;
        _logger = logger;
    }

    [BindProperty]
    public CreatePersonViewModel Person { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public IActionResult OnGet()
    {
        // Initialize with default values
        Person = new CreatePersonViewModel();
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var id = await _peopleService.CreatePersonAsync(Person);
            StatusMessage = "Person was created successfully.";
            return RedirectToPage("./Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new person");
            StatusMessage = "Error: Failed to create person.";
            return Page();
        }
    }
}

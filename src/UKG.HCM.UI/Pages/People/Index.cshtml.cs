using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UKG.HCM.UI.Extensions;
using UKG.HCM.UI.Models;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Pages.People;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IPeopleService peopleService, ILogger<IndexModel> logger)
    {
        _peopleService = peopleService;
        _logger = logger;
    }

    public IEnumerable<PersonViewModel> People { get; private set; } = Array.Empty<PersonViewModel>();
    
    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            if (User.IsInRole(UKG.HCM.Shared.Constants.ApplicationRoles.HRAdmin) || 
                User.IsInRole(UKG.HCM.Shared.Constants.ApplicationRoles.Manager))
            {
                // Admins and Managers can see all people
                People = await _peopleService.GetPeopleAsync();
            }
            else
            {
                // Regular employees can only see their own record
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var allPeople = await _peopleService.GetPeopleAsync();
                    // Filter by the user's email
                    People = allPeople.Where(p => p.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
                }
            }
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving people");
            StatusMessage = "Error: Failed to retrieve people records.";
            return Page();
        }
    }
}
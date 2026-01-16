using BLL;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Tours;

public class CreateModel : PageModel
{
    private readonly IRepository<Tour> _tourRepository;

    public CreateModel(IRepository<Tour> tourRepository)
    {
        _tourRepository = tourRepository;
    }

    [BindProperty]
    public Tour Tour { get; set; } = new();

    public IActionResult OnGet()
    {
        Tour.StartTime = DateTime.Now.AddDays(1); // Default to tomorrow
        Tour.Duration = TimeSpan.FromHours(2); // Default 2 hours
        Tour.Capacity = 10; // Default capacity
        Tour.PricePerParticipant = 25; // Default price per participant
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _tourRepository.AddAsync(Tour);
        await _tourRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }
}

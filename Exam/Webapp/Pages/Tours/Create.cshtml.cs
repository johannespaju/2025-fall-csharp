using BLL;
using BLL.Enums;
using BLL.Interfaces;using Microsoft.AspNetCore.Mvc;
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
        Tour.DurationHours = 2; // Default 2 hours
        Tour.MaxCapacity = 10; // Default capacity
        Tour.IncludedBikeType = BikeType.City; // Default bike type
        Tour.UpgradeToElectricFee = 10; // Default upgrade fee
        Tour.IsActive = true;
        Tour.TimeSlots = "09:00,14:00"; // Initialize with default time slots (comma-separated)
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

using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Bikes;

public class CreateModel : PageModel
{
    private readonly IRepository<Bike> _bikeRepository;

    public CreateModel(IRepository<Bike> bikeRepository)
    {
        _bikeRepository = bikeRepository;
    }

    [BindProperty]
    public Bike Bike { get; set; } = new();

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Auto-populate DailyRate and ServiceInterval based on bike type
        (Bike.DailyRate, Bike.ServiceInterval) = Bike.Type switch
        {
            BikeType.City => (12.00m, 500),
            BikeType.Electric => (28.00m, 300),
            BikeType.Mountain => (18.00m, 400),
            BikeType.Tandem => (22.00m, 500),
            BikeType.Children => (8.00m, 500),
            _ => (12.00m, 500)
        };

        // Remove validation errors for auto-populated fields
        ModelState.Remove("Bike.DailyRate");
        ModelState.Remove("Bike.ServiceInterval");

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return Page();
        }

        await _bikeRepository.AddAsync(Bike);
        await _bikeRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Bike {Bike.BikeNumber} created successfully!";
        return RedirectToPage(nameof(Index));
    }
}

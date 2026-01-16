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

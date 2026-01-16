using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Bikes;

public class EditModel : PageModel
{
    private readonly IRepository<Bike> _bikeRepository;

    public EditModel(IRepository<Bike> bikeRepository)
    {
        _bikeRepository = bikeRepository;
    }

    [BindProperty]
    public Bike Bike { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var bike = await _bikeRepository.GetByIdAsync(id);
        if (bike == null)
        {
            return NotFound();
        }

        Bike = bike;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return Page();
        }

        await _bikeRepository.UpdateAsync(Bike);
        await _bikeRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Bike {Bike.BikeNumber} updated successfully!";
        return RedirectToPage(nameof(Index));
    }
}

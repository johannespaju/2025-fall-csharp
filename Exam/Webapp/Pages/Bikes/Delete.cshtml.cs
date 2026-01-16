using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Bikes;

public class DeleteModel : PageModel
{
    private readonly IRepository<Bike> _bikeRepository;

    public DeleteModel(IRepository<Bike> bikeRepository)
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
        await _bikeRepository.DeleteAsync(Bike);
        await _bikeRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Bike {Bike.BikeNumber} deleted successfully!";
        return RedirectToPage(nameof(Index));
    }
}

using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Reservations;

public class DeleteModel : PageModel
{
    private readonly IRepository<Rental> _rentalRepository;

    public DeleteModel(IRepository<Rental> rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    [BindProperty]
    public Rental Rental { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Rental = await _rentalRepository.GetByIdAsync(id);
        if (Rental == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _rentalRepository.DeleteAsync(Rental);
        await _rentalRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }
}

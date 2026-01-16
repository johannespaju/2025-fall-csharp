using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Tours;

public class EditModel : PageModel
{
    private readonly IRepository<Tour> _tourRepository;

    public EditModel(IRepository<Tour> tourRepository)
    {
        _tourRepository = tourRepository;
    }

    [BindProperty]
    public Tour Tour { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var tour = await _tourRepository.GetByIdAsync(id);
        if (tour == null)
        {
            return NotFound();
        }

        Tour = tour;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _tourRepository.UpdateAsync(Tour);
        await _tourRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }
}

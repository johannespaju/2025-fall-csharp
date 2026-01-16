using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Webapp.Pages.Rentals;

public class EditModel : PageModel
{
    private readonly IRepository<Rental> _rentalRepository;
    private readonly IRepository<Bike> _bikeRepository;
    private readonly IRepository<Customer> _customerRepository;

    public EditModel(
        IRepository<Rental> rentalRepository,
        IRepository<Bike> bikeRepository,
        IRepository<Customer> customerRepository)
    {
        _rentalRepository = rentalRepository;
        _bikeRepository = bikeRepository;
        _customerRepository = customerRepository;
    }

    [BindProperty]
    public Rental Rental { get; set; } = new();

    public List<SelectListItem> BikeOptions { get; set; } = new();
    public List<SelectListItem> CustomerOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Rental = await _rentalRepository.GetByIdAsync(id);
        if (Rental == null)
        {
            return NotFound();
        }

        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        await _rentalRepository.UpdateAsync(Rental);
        await _rentalRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }

    private async Task LoadOptionsAsync()
    {
        var bikes = await _bikeRepository.GetAllAsync();
        BikeOptions = bikes.Select(b => new SelectListItem
        {
            Value = b.Id.ToString(),
            Text = $"{b.Type} (€{b.DailyRate}/day)"
        }).ToList();

        var customers = await _customerRepository.GetAllAsync();
        CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.FirstName} {c.LastName}"
        }).ToList();
    }
}

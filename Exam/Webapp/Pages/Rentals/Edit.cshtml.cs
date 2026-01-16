using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Webapp.Pages.Rentals;

public class EditModel : PageModel
{
    private static readonly TimeOnly[] ValidStartTimes =
    {
        new(9, 0),
        new(13, 0),
        new(17, 0),
        new(21, 0)
    };

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
    public List<SelectListItem> StartTimeOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Rental = await _rentalRepository.GetByIdAsync(id);
        if (Rental == null)
        {
            return NotFound();
        }

        await LoadOptionsAsync();
        LoadStartTimeOptions();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        SetRentalEndFromType();
        ModelState.Clear();
        TryValidateModel(Rental);

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            LoadStartTimeOptions();
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

    private void LoadStartTimeOptions()
    {
        StartTimeOptions = ValidStartTimes
            .Select(time => new SelectListItem
            {
                Value = time.ToString("HH:mm"),
                Text = time.ToString("HH:mm")
            })
            .ToList();
    }

    private void SetRentalEndFromType()
    {
        var startDateTime = Rental.StartDate.ToDateTime(Rental.StartTime);
        var duration = Rental.RentalType == RentalType.FullDay
            ? TimeSpan.FromHours(24)
            : TimeSpan.FromHours(4);

        var endDateTime = startDateTime.Add(duration);
        Rental.EndDate = DateOnly.FromDateTime(endDateTime);
        Rental.EndTime = TimeOnly.FromDateTime(endDateTime);
    }
}

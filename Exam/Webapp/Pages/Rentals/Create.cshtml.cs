using BLL;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Webapp.Pages.Rentals;

public class CreateModel : PageModel
{
    private readonly IRepository<Rental> _rentalRepository;
    private readonly IRepository<Bike> _bikeRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly IPricingService _pricingService;
    private readonly IDepositService _depositService;

    public CreateModel(
        IRepository<Rental> rentalRepository,
        IRepository<Bike> bikeRepository,
        IRepository<Customer> customerRepository,
        IAvailabilityService availabilityService,
        IPricingService pricingService,
        IDepositService depositService)
    {
        _rentalRepository = rentalRepository;
        _bikeRepository = bikeRepository;
        _customerRepository = customerRepository;
        _availabilityService = availabilityService;
        _pricingService = pricingService;
        _depositService = depositService;
    }

    [BindProperty]
    public Rental Rental { get; set; } = new();

    public List<SelectListItem> BikeOptions { get; set; } = new();
    public List<SelectListItem> CustomerOptions { get; set; } = new();
    public decimal CalculatedPrice { get; set; }
    public decimal CalculatedDeposit { get; set; }

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
        Rental.StartTime = DateTime.Now;
        Rental.EndTime = DateTime.Now.AddHours(4);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        // Check availability
        var isAvailable = await _availabilityService.IsBikeAvailableAsync(
            Rental.BikeId, Rental.StartTime, Rental.EndTime);

        if (!isAvailable)
        {
            ModelState.AddModelError(string.Empty, "Selected bike is not available for the requested time period.");
            await LoadOptionsAsync();
            return Page();
        }

        // Calculate price and deposit
        var bike = await _bikeRepository.GetByIdAsync(Rental.BikeId);
        var deposit = await _depositService.CalculateDepositAsync(Rental.BikeId, Rental.CustomerId);
        var price = _pricingService.CalculateRentalPrice(
            Rental.StartTime, Rental.EndTime, bike!.DailyRate);

        // Create rental
        Rental.TotalPrice = price;
        Rental.Deposit = deposit;
        Rental.IsActive = true;

        await _rentalRepository.AddAsync(Rental);
        await _rentalRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }

    public async Task<IActionResult> OnGetCalculateAsync(
        Guid bikeId, Guid customerId, DateTime startTime, DateTime endTime)
    {
        var bike = await _bikeRepository.GetByIdAsync(bikeId);
        var price = _pricingService.CalculateRentalPrice(startTime, endTime, bike!.DailyRate);
        var deposit = await _depositService.CalculateDepositAsync(bikeId, customerId);

        return new JsonResult(new
        {
            price = price.ToString("C"),
            deposit = deposit.ToString("C")
        });
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

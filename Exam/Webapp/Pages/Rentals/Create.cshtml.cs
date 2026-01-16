using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Webapp.Pages.Rentals;

public class CreateModel : PageModel
{
    private static readonly TimeOnly[] ValidStartTimes =
    {
        new(9, 0),
        new(13, 0),
        new(17, 0),
    };

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

    [BindProperty]
    public BikeType SelectedBikeType { get; set; }

    public string? AssignedBikeNumber { get; set; }
    public List<SelectListItem> CustomerOptions { get; set; } = new();
    public List<SelectListItem> StartTimeOptions { get; set; } = new();
    public decimal CalculatedPrice { get; set; }
    public decimal CalculatedDeposit { get; set; }

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
        LoadStartTimeOptions();
        var now = DateTime.Now;
        Rental.StartDate = DateOnly.FromDateTime(now);
        Rental.StartTime = GetNextValidStartTime(TimeOnly.FromDateTime(now));
        Rental.RentalType = RentalType.FourHour;
        Rental.Status = RentalStatus.Reserved;
        SetRentalEndFromType();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        SetRentalEndFromType();

        // Remove BikeId validation errors since we'll auto-assign
        ModelState.Remove("Rental.BikeId");
        ModelState.Clear();
        TryValidateModel(Rental);

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            LoadStartTimeOptions();
            return Page();
        }

        // Convert DateOnly/TimeOnly to DateTime for availability check
        var startTime = Rental.StartDate.ToDateTime(Rental.StartTime);
        var endTime = Rental.EndDate.ToDateTime(Rental.EndTime);

        // Find available bikes of the selected type
        var availableBikeIds = await _availabilityService.GetAvailableBikesAsync(startTime, endTime);
        var allBikes = await _bikeRepository.GetAllAsync();

        var availableBike = allBikes
            .Where(b => availableBikeIds.Contains(b.Id) && b.Type == SelectedBikeType)
            .OrderBy(b => b.BikeNumber)
            .FirstOrDefault();

        if (availableBike == null)
        {
            ModelState.AddModelError(string.Empty, $"No {SelectedBikeType} bikes are available for the requested time period.");
            await LoadOptionsAsync();
            LoadStartTimeOptions();
            return Page();
        }

        // Assign the bike
        Rental.BikeId = availableBike.Id;

        // Calculate price and deposit
        var deposit = await _depositService.CalculateDepositAsync(availableBike.Id, Rental.CustomerId);
        var price = _pricingService.CalculateRentalPrice(
            startTime, endTime, availableBike.DailyRate);

        // Create rental
        Rental.TotalCost = price;
        Rental.DepositAmount = deposit;
        Rental.Status = RentalStatus.Active;

        await _rentalRepository.AddAsync(Rental);
        await _rentalRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Rental created successfully! Assigned bike: {availableBike.BikeNumber}";
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
        var customers = await _customerRepository.GetAllAsync();
        CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.FirstName} {c.LastName}"
        }).ToList();

        // Store damage counts for JavaScript
        CustomerDamageCounts = customers.ToDictionary(
            c => c.Id.ToString(),
            c => c.DamageIncidentCount);
    }

    public Dictionary<string, int> CustomerDamageCounts { get; set; } = new();

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

    private static TimeOnly GetNextValidStartTime(TimeOnly currentTime)
    {
        foreach (var validTime in ValidStartTimes)
        {
            if (validTime >= currentTime)
            {
                return validTime;
            }
        }

        return ValidStartTimes[0];
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

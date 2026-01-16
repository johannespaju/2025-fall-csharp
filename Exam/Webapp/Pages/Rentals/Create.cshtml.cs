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
        new(21, 0)
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

    public List<SelectListItem> BikeOptions { get; set; } = new();
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
        ModelState.Clear();
        TryValidateModel(Rental);

        Console.WriteLine($"POST received - BikeId: {Rental.BikeId}, CustomerId: {Rental.CustomerId}");
        Console.WriteLine($"Dates: {Rental.StartDate} {Rental.StartTime} to {Rental.EndDate} {Rental.EndTime}");
        Console.WriteLine($"RentalType: {Rental.RentalType}, Status: {Rental.Status}");
        Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
        
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }
            await LoadOptionsAsync();
            LoadStartTimeOptions();
            return Page();
        }

        // Convert DateOnly/TimeOnly to DateTime for availability check
        var startTime = Rental.StartDate.ToDateTime(Rental.StartTime);
        var endTime = Rental.EndDate.ToDateTime(Rental.EndTime);

        // Check availability
        var isAvailable = await _availabilityService.IsBikeAvailableAsync(
            Rental.BikeId, startTime, endTime);

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
            startTime, endTime, bike!.DailyRate);

        // Create rental
        Rental.TotalCost = price;
        Rental.DepositAmount = deposit;
        Rental.Status = RentalStatus.Active;

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
            Text = $"{b.BikeNumber} - {b.Type} (€{b.DailyRate}/day)"
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

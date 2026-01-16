using BLL;
using BLL.Enums;
using BLL.Interfaces;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Webapp.Pages.Tours;

public class BookModel : PageModel
{
    private readonly IRepository<Tour> _tourRepository;
    private readonly IRepository<TourBooking> _tourBookingRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Rental> _rentalRepository;
    private readonly IRepository<Bike> _bikeRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly IPricingService _pricingService;
    private readonly IDepositService _depositService;
    private readonly AppDbContext _context;

    public BookModel(
        IRepository<Tour> tourRepository,
        IRepository<TourBooking> tourBookingRepository,
        IRepository<Customer> customerRepository,
        IRepository<Rental> rentalRepository,
        IRepository<Bike> bikeRepository,
        IAvailabilityService availabilityService,
        IPricingService pricingService,
        IDepositService depositService,
        AppDbContext context)
    {
        _tourRepository = tourRepository;
        _tourBookingRepository = tourBookingRepository;
        _customerRepository = customerRepository;
        _rentalRepository = rentalRepository;
        _bikeRepository = bikeRepository;
        _availabilityService = availabilityService;
        _pricingService = pricingService;
        _depositService = depositService;
        _context = context;
    }

    public Tour Tour { get; set; } = new();

    [BindProperty]
    public TourBooking TourBooking { get; set; } = new();

    public List<SelectListItem> CustomerOptions { get; set; } = new();
    public List<SelectListItem> TimeSlotOptions { get; set; } = new();
    public int AvailableSlots { get; set; }
    public decimal CalculatedTotalPrice { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Tour = await _tourRepository.GetByIdAsync(id);
        if (Tour == null)
        {
            return NotFound();
        }

        var customers = await _customerRepository.GetAllAsync();
        CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.FirstName} {c.LastName}"
        }).ToList();

        TourBooking.ParticipantCount = 1;
        TourBooking.TourId = Tour.Id;
        TourBooking.BookingDate = DateOnly.FromDateTime(DateTime.Now);
        LoadTimeSlots();
        if (TimeSlotOptions.Count > 0)
        {
            TourBooking.TimeSlot = TimeOnly.Parse(TimeSlotOptions[0].Value);
        }
        TourBooking.Status = TourBookingStatus.Confirmed;
        
        await CalculateAvailableSlotsAsync();
        CalculateTotalPrice();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (TourBooking.TourId == Guid.Empty)
        {
            ModelState.AddModelError(string.Empty, "Tour information is missing.");
        }

        if (!await LoadTourAsync(TourBooking.TourId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            LoadTimeSlots();
            await CalculateAvailableSlotsAsync();
            CalculateTotalPrice();
            return Page();
        }

        // Check if there are available slots
        await CalculateAvailableSlotsAsync();
        if (TourBooking.ParticipantCount > AvailableSlots)
        {
            ModelState.AddModelError(string.Empty, "Not enough available slots for this tour.");
            await LoadOptionsAsync();
            LoadTimeSlots();
            await CalculateAvailableSlotsAsync();
            CalculateTotalPrice();
            return Page();
        }

        // Calculate total price (use new TotalCost property)
        CalculateTotalPrice();
        TourBooking.TourId = Tour.Id;
        TourBooking.CreatedAt = DateTime.Now;
        TourBooking.Status = TourBookingStatus.Confirmed;

        var tourStartDateTime = TourBooking.BookingDate.ToDateTime(TourBooking.TimeSlot);
        var tourEndDateTime = tourStartDateTime.AddHours((double)Tour.DurationHours);

        // Get available bikes for the tour
        var availableBikeIds = await _availabilityService.GetAvailableBikesAsync(
            tourStartDateTime, tourEndDateTime);

        // If not enough bikes available,reject booking
        if (availableBikeIds.Count() < TourBooking.ParticipantCount)
        {
            ModelState.AddModelError(string.Empty, "Not enough bikes available for this tour.");
            await LoadOptionsAsync();
            LoadTimeSlots();
            await CalculateAvailableSlotsAsync();
            CalculateTotalPrice();
            return Page();
        }

        // Create tour booking
        await _tourBookingRepository.AddAsync(TourBooking);
        await _tourBookingRepository.SaveChangesAsync();

        // Create individual rentals for each participant
        var bikes = await _bikeRepository.GetAllAsync();
        foreach (var bikeId in availableBikeIds.Take(TourBooking.ParticipantCount))
        {
            var bike = bikes.First(b => b.Id == bikeId);
            var deposit = await _depositService.CalculateDepositAsync(bikeId, TourBooking.CustomerId);
            var price = _pricingService.CalculateRentalPrice(
                tourStartDateTime, tourEndDateTime, bike.DailyRate);

            var rental = new Rental
            {
                BikeId = bikeId,
                CustomerId = TourBooking.CustomerId,
                TourBookingId = TourBooking.Id,
                StartDate = DateOnly.FromDateTime(tourStartDateTime),
                StartTime = TimeOnly.FromDateTime(tourStartDateTime),
                EndDate = DateOnly.FromDateTime(tourEndDateTime),
                EndTime = TimeOnly.FromDateTime(tourEndDateTime),
                TotalCost = price,
                DepositAmount = deposit,
                Status = RentalStatus.Active,
                RentalType = RentalType.FullDay  // Tours use FullDay rental type
            };

            await _rentalRepository.AddAsync(rental);
            bike.Status = BikeStatus.Rented;
            await _bikeRepository.UpdateAsync(bike);
        }

        await _rentalRepository.SaveChangesAsync();
        await _bikeRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }

    private async Task LoadOptionsAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.FirstName} {c.LastName}"
        }).ToList();
    }

    private async Task<int> GetBookedParticipantCountAsync(Guid tourId, DateOnly date, TimeOnly timeSlot)
    {
        return await _context.TourBookings
            .Where(tb => tb.TourId == tourId
                      && tb.BookingDate == date
                      && tb.TimeSlot == timeSlot
                      && tb.Status != TourBookingStatus.Cancelled)
            .SumAsync(tb => tb.ParticipantCount);
    }

    private async Task CalculateAvailableSlotsAsync()
    {
        var bookedParticipants = await GetBookedParticipantCountAsync(Tour.Id, TourBooking.BookingDate, TourBooking.TimeSlot);
        AvailableSlots = Math.Max(0, Tour.MaxCapacity - bookedParticipants);
    }

    private void CalculateTotalPrice()
    {
        var pricePerParticipant = Tour.PricePerParticipant;
        if (TourBooking.BikeUpgradeToElectric)
        {
            pricePerParticipant += Tour.UpgradeToElectricFee;
        }

        CalculatedTotalPrice = TourBooking.ParticipantCount * pricePerParticipant;
        TourBooking.TotalCost = CalculatedTotalPrice;
    }

    private async Task<bool> LoadTourAsync(Guid tourId)
    {
        Tour = await _tourRepository.GetByIdAsync(tourId);
        if (Tour != null)
        {
            LoadTimeSlots();
        }
        return Tour != null;
    }

    private void LoadTimeSlots()
    {
        TimeSlotOptions = (Tour.TimeSlots ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(slot => new SelectListItem
            {
                Value = slot,
                Text = slot
            })
            .ToList();
    }
}

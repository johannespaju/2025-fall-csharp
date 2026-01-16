using BLL.Enums;
using BLL.Interfaces;

namespace BLL.Services;

public class TourService : ITourService
{
    private readonly IRepository<Tour> _tourRepo;
    private readonly IRepository<TourBooking> _bookingRepo;
    private readonly IRepository<Rental> _rentalRepo;
    private readonly IRepository<Bike> _bikeRepo;
    private readonly IRepository<Customer> _customerRepo;
    private readonly IAvailabilityService _availabilityService;
    private readonly IPricingService _pricingService;
    private readonly IDepositService _depositService;
    
    public TourService(
        IRepository<Tour> tourRepo,
        IRepository<TourBooking> bookingRepo,
        IRepository<Rental> rentalRepo,
        IRepository<Bike> bikeRepo,
        IRepository<Customer> customerRepo,
        IAvailabilityService availabilityService,
        IPricingService pricingService,
        IDepositService depositService)
    {
        _tourRepo = tourRepo;
        _bookingRepo = bookingRepo;
        _rentalRepo = rentalRepo;
        _bikeRepo = bikeRepo;
        _customerRepo = customerRepo;
        _availabilityService = availabilityService;
        _pricingService = pricingService;
        _depositService = depositService;
    }
    
    public async Task<int> GetRemainingCapacityAsync(Guid tourId, DateOnly date, TimeOnly timeSlot)
    {
        var tour = await _tourRepo.GetByIdAsync(tourId);
        if (tour == null) return 0;
        
        var allBookings = await _bookingRepo.GetAllAsync();
        var bookedCount = allBookings
            .Where(tb => tb.TourId == tourId
                      && tb.BookingDate == date
                      && tb.TimeSlot == timeSlot
                      && tb.Status != TourBookingStatus.Cancelled)
            .Sum(tb => tb.ParticipantCount);
        
        return Math.Max(0, tour.MaxCapacity - bookedCount);
    }
    
    public async Task<bool> ValidateTourBookingAsync(
        Guid tourId, DateOnly date, TimeOnly timeSlot, int participantCount)
    {
        var remaining = await GetRemainingCapacityAsync(tourId, date, timeSlot);
        return participantCount <= remaining;
    }
    
    public async Task<TourBooking> CreateTourBookingWithRentalsAsync(
        Guid tourId, Guid customerId, DateOnly date, TimeOnly timeSlot, 
        int participantCount, bool upgradeToElectric)
    {
        // 1. Get entities
        var tour = await _tourRepo.GetByIdAsync(tourId);
        if (tour == null) 
            throw new ArgumentException("Tour not found", nameof(tourId));
            
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null)
            throw new ArgumentException("Customer not found", nameof(customerId));
        
        // 2. Validate capacity
        if (!await ValidateTourBookingAsync(tourId, date, timeSlot, participantCount))
            throw new InvalidOperationException("Insufficient tour capacity");
        
        // 3. Create TourBooking
        var tourBooking = new TourBooking
        {
            TourId = tourId,
            CustomerId = customerId,
            BookingDate = date,
            TimeSlot = timeSlot,
            ParticipantCount = participantCount,
            BikeUpgradeToElectric = upgradeToElectric,
            Status = TourBookingStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };
        
        await _bookingRepo.AddAsync(tourBooking);
        await _bookingRepo.SaveChangesAsync();
        
        // 4. Get N available bikes (City or Electric based on upgrade)
        var bikeType = upgradeToElectric ? BikeType.Electric : BikeType.City;
        var endTime = timeSlot.AddHours((double)tour.DurationHours);
        
        var startDateTime = date.ToDateTime(timeSlot);
        var endDateTime = date.ToDateTime(endTime);
        
        var allBikes = await _bikeRepo.GetAllAsync();
        var candidateBikes = allBikes
            .Where(b => b.Type == bikeType && b.Status == BikeStatus.Available)
            .ToList();
        
        var availableBikes = new List<Bike>();
        foreach (var bike in candidateBikes)
        {
            if (await _availabilityService.IsBikeAvailableAsync(
                bike.Id, startDateTime, endDateTime))
            {
                availableBikes.Add(bike);
                if (availableBikes.Count >= participantCount)
                    break;
            }
        }
        
        if (availableBikes.Count < participantCount)
            throw new InvalidOperationException(
                $"Only {availableBikes.Count} {bikeType} bikes available");
        
        // 5. Create N Rental records (one per participant)
        decimal totalCost = 0;
        foreach (var bike in availableBikes.Take(participantCount))
        {
            var rentalCost = _pricingService.CalculateRentalCost(
                bike.Type, tour.DurationHours);
            var deposit = await _depositService.CalculateDepositAsync(
                bike.Id, customerId);
                
            var rental = new Rental
            {
                BikeId = bike.Id,
                CustomerId = customerId,
                TourBookingId = tourBooking.Id,
                StartDate = date,
                StartTime = timeSlot,
                EndDate = date,
                EndTime = endTime,
                RentalType = RentalType.FullDay,
                DepositAmount = deposit,
                TotalCost = rentalCost,
                Status = RentalStatus.Reserved
            };
            
            await _rentalRepo.AddAsync(rental);
            totalCost += rentalCost;
        }
        
        // 6. Update TourBooking.TotalCost
        tourBooking.TotalCost = totalCost;
        await _bookingRepo.UpdateAsync(tourBooking);
        await _bookingRepo.SaveChangesAsync();
        
        return tourBooking;
    }
}

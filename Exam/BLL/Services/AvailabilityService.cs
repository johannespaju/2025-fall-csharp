using BLL.Interfaces;
using BLL.Enums;

namespace BLL.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IRepository<Bike> _bikeRepository;
    private readonly IRepository<Rental> _rentalRepository;
    private readonly IMaintenanceService _maintenanceService;

    public AvailabilityService(IRepository<Bike> bikeRepository, IRepository<Rental> rentalRepository, 
        IMaintenanceService maintenanceService)
    {
        _bikeRepository = bikeRepository;
        _rentalRepository = rentalRepository;
        _maintenanceService = maintenanceService;
    }

    public async Task<bool> IsBikeAvailableAsync(Guid bikeId, DateTime startTime, DateTime endTime)
    {
        var bike = await _bikeRepository.GetByIdAsync(bikeId);
        if (bike == null || bike.Status != BikeStatus.Available)
            return false;

        if (await _maintenanceService.IsBikeDueForMaintenanceAsync(bikeId))
            return false;

        var rentals = await _rentalRepository.GetAllAsync();
        var activeRentals = rentals.Where(r => r.Bike != null && r.Bike.Id == bikeId && r.Status != RentalStatus.Cancelled);

        return !activeRentals.Any(r =>
        {
            var rentalStart = r.StartDate.ToDateTime(r.StartTime);
            var rentalEnd = r.EndDate.ToDateTime(r.EndTime);
            return DoTimeIntervalsOverlap(rentalStart, rentalEnd, startTime, endTime);
        });
    }

    public async Task<IEnumerable<Guid>> GetAvailableBikesAsync(DateTime startTime, DateTime endTime)
    {
        var bikes = await _bikeRepository.GetAllAsync();
        var availableBikes = new List<Guid>();

        foreach (var bike in bikes.Where(b => b.Status == BikeStatus.Available))
        {
            if (!await _maintenanceService.IsBikeDueForMaintenanceAsync(bike.Id) &&
                !await HasActiveRentalsOverlappingAsync(bike.Id, startTime, endTime))
            {
                availableBikes.Add(bike.Id);
            }
        }

        return availableBikes;
    }

    public async Task<bool> IsRentalExtendableAsync(Guid rentalId, DateTime newEndTime)
    {
        var rentals = await _rentalRepository.GetAllAsync();
        var rental = rentals.FirstOrDefault(r => r.Id == rentalId);
        
        if (rental == null || rental.Status != RentalStatus.Active)
            return false;

        var currentEndTime = rental.EndDate.ToDateTime(rental.EndTime);
        if (newEndTime <= currentEndTime)
            return false;

        var rentalStart = rental.StartDate.ToDateTime(rental.StartTime);
        
        var overlappingRentals = rentals.Where(r =>
            r.Bike != null && rental.Bike != null &&
            r.Bike.Id == rental.Bike.Id &&
            r.Status != RentalStatus.Cancelled &&
            r.Id != rentalId)
            .Select(r => new
            {
                Rental = r,
                Start = r.StartDate.ToDateTime(r.StartTime),
                End = r.EndDate.ToDateTime(r.EndTime)
            })
            .Where(r => DoTimeIntervalsOverlap(rentalStart, newEndTime, r.Start, r.End));

        return !overlappingRentals.Any();
    }

    private async Task<bool> HasActiveRentalsOverlappingAsync(Guid bikeId, DateTime startTime, DateTime endTime)
    {
        var rentals = await _rentalRepository.GetAllAsync();
        var activeRentals = rentals.Where(r => r.Bike != null && r.Bike.Id == bikeId && r.Status != RentalStatus.Cancelled);

        return activeRentals.Any(r =>
        {
            var rentalStart = r.StartDate.ToDateTime(r.StartTime);
            var rentalEnd = r.EndDate.ToDateTime(r.EndTime);
            return DoTimeIntervalsOverlap(rentalStart, rentalEnd, startTime, endTime);
        });
    }

    private bool DoTimeIntervalsOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        return start1 < end2 && start2 < end1;
    }
}

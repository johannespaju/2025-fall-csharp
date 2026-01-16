using BLL.Interfaces;

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
        if (bike == null || !bike.IsAvailable)
            return false;

        if (await _maintenanceService.IsBikeDueForMaintenanceAsync(bikeId))
            return false;

        var rentals = await _rentalRepository.GetAllAsync();
        var activeRentals = rentals.Where(r => r.BikeId == bikeId && r.IsActive);

        return !activeRentals.Any(r => DoTimeIntervalsOverlap(r.StartTime, r.EndTime, startTime, endTime));
    }

    public async Task<IEnumerable<Guid>> GetAvailableBikesAsync(DateTime startTime, DateTime endTime)
    {
        var bikes = await _bikeRepository.GetAllAsync();
        var availableBikes = new List<Guid>();

        foreach (var bike in bikes.Where(b => b.IsAvailable))
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
        
        if (rental == null || !rental.IsActive || newEndTime <= rental.EndTime)
            return false;

        var overlappingRentals = rentals.Where(r => 
            r.BikeId == rental.BikeId && 
            r.IsActive && 
            r.Id != rentalId && 
            DoTimeIntervalsOverlap(rental.StartTime, newEndTime, r.StartTime, r.EndTime));

        return !overlappingRentals.Any();
    }

    private async Task<bool> HasActiveRentalsOverlappingAsync(Guid bikeId, DateTime startTime, DateTime endTime)
    {
        var rentals = await _rentalRepository.GetAllAsync();
        var activeRentals = rentals.Where(r => r.BikeId == bikeId && r.IsActive);

        return activeRentals.Any(r => DoTimeIntervalsOverlap(r.StartTime, r.EndTime, startTime, endTime));
    }

    private bool DoTimeIntervalsOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        return start1 < end2 && start2 < end1;
    }
}

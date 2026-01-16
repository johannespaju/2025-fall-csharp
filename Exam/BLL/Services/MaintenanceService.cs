using BLL.Interfaces;
using BLL.Enums;

namespace BLL.Services;

public class MaintenanceService : IMaintenanceService
{
    private const int MaintenanceBuffer = 50; // 50km buffer before service interval

    private readonly IRepository<Bike> _bikeRepository;
    private readonly IRepository<MaintenanceRecord> _maintenanceRepository;

    public MaintenanceService(IRepository<Bike> bikeRepository, IRepository<MaintenanceRecord> maintenanceRepository)
    {
        _bikeRepository = bikeRepository;
        _maintenanceRepository = maintenanceRepository;
    }

    public async Task<bool> IsBikeDueForMaintenanceAsync(Guid bikeId)
    {
        var bike = await _bikeRepository.GetByIdAsync(bikeId);
        if (bike == null)
            return false;

        var distanceToService = await CalculateRemainingDistanceToServiceAsync(bikeId);
        return distanceToService <= MaintenanceBuffer;
    }

    public async Task<int> CalculateRemainingDistanceToServiceAsync(Guid bikeId)
    {
        var bike = await _bikeRepository.GetByIdAsync(bikeId);
        if (bike == null)
            return 0;

        var maintenanceRecords = await _maintenanceRepository.GetAllAsync();
        var lastMaintenance = maintenanceRecords
            .Where(m => m.Bike.Id == bikeId)
            .OrderByDescending(m => m.ScheduledDate)
            .FirstOrDefault();

        if (lastMaintenance == null)
            return bike.ServiceInterval - bike.CurrentOdometer;

        var distanceSinceLastService = bike.CurrentOdometer - lastMaintenance.OdometerAtService;
        return bike.ServiceInterval - distanceSinceLastService;
    }

    public async Task FlagBikeForMaintenanceAsync(Guid bikeId)
    {
        var bike = await _bikeRepository.GetByIdAsync(bikeId);
        if (bike != null)
        {
            bike.Status = BikeStatus.InMaintenance;
            await _bikeRepository.UpdateAsync(bike);
            await _bikeRepository.SaveChangesAsync();
        }
    }

    public async Task RecordMaintenanceAsync(Guid bikeId, DateTime date, decimal cost, int odometerAtService, string? description)
    {
        // Note: BikeId is int in MaintenanceRecord, need to find the bike first to get its int ID
        var bike = await _bikeRepository.GetByIdAsync(bikeId);
        if (bike == null)
            return;

        var maintenanceRecord = new MaintenanceRecord
        {
            Bike = bike,
            ScheduledDate = date,
            CompletedAt = DateTime.UtcNow,
            Cost = cost,
            OdometerAtService = odometerAtService,
            Description = description
        };

        await _maintenanceRepository.AddAsync(maintenanceRecord);
        await _maintenanceRepository.SaveChangesAsync();

        bike.Status = BikeStatus.Available;
        bike.LastServiceOdometer = odometerAtService;
        await _bikeRepository.UpdateAsync(bike);
        await _bikeRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<Guid>> GetBikesDueForMaintenanceAsync()
    {
        var bikes = await _bikeRepository.GetAllAsync();
        var bikesDueForMaintenance = new List<Guid>();

        foreach (var bike in bikes)
        {
            if (await IsBikeDueForMaintenanceAsync(bike.Id))
            {
                bikesDueForMaintenance.Add(bike.Id);
            }
        }

        return bikesDueForMaintenance;
    }
}

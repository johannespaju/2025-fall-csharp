namespace BLL.Interfaces;

public interface IMaintenanceService
{
    Task<bool> IsBikeDueForMaintenanceAsync(Guid bikeId);
    Task<int> CalculateRemainingDistanceToServiceAsync(Guid bikeId);
    Task FlagBikeForMaintenanceAsync(Guid bikeId);
    Task RecordMaintenanceAsync(Guid bikeId, DateTime date, decimal cost, int odometerAtService, string? description);
    Task<IEnumerable<Guid>> GetBikesDueForMaintenanceAsync();
}

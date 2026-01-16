namespace BLL.Interfaces;

public interface IAvailabilityService
{
    Task<bool> IsBikeAvailableAsync(Guid bikeId, DateTime startTime, DateTime endTime);
    Task<IEnumerable<Guid>> GetAvailableBikesAsync(DateTime startTime, DateTime endTime);
    Task<bool> IsRentalExtendableAsync(Guid rentalId, DateTime newEndTime);
}

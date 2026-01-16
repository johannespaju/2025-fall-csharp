namespace BLL.Interfaces;

public interface ITourService
{
    Task<int> GetRemainingCapacityAsync(Guid tourId, DateOnly date, TimeOnly timeSlot);
    
    Task<bool> ValidateTourBookingAsync(
        Guid tourId, DateOnly date, TimeOnly timeSlot, int participantCount);
    
    Task<TourBooking> CreateTourBookingWithRentalsAsync(
        Guid tourId,
        Guid customerId,
        DateOnly date,
        TimeOnly timeSlot,
        int participantCount,
        bool upgradeToElectric);
}

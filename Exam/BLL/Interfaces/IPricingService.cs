namespace BLL.Interfaces;

public interface IPricingService
{
    decimal CalculateRentalPrice(DateTime startTime, DateTime endTime, decimal dailyRate);
    decimal CalculateTourPrice(int participantCount, decimal basePricePerPerson);
    decimal CalculateUpgradeFee(decimal originalDailyRate, decimal newDailyRate, DateTime startTime, DateTime endTime);
    decimal CalculateRentalCost(BikeType bikeType, decimal durationHours);
}

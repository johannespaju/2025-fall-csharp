using BLL.Interfaces;
using BLL.Enums;

namespace BLL.Services;

public class PricingService : IPricingService
{
    private const int HoursPerDay = 24;
    private const int FourHourBlock = 4;
    private const decimal FourHourBlockRate = 0.5m; // 50% of daily rate

    public decimal CalculateRentalPrice(DateTime startTime, DateTime endTime, decimal dailyRate)
    {
        var duration = endTime - startTime;
        var totalHours = (decimal)duration.TotalHours;

        // Check if it's a full-day rental (24 hours or more)
        if (totalHours >= HoursPerDay)
        {
            var fullDays = Math.Ceiling(totalHours / HoursPerDay);
            return fullDays * dailyRate;
        }

        // Calculate price for 4-hour blocks
        var blocks = Math.Ceiling(totalHours / FourHourBlock);
        var blockPrice = dailyRate * FourHourBlockRate;
        
        return blocks * blockPrice;
    }

    public decimal CalculateTourPrice(int participantCount, decimal basePricePerPerson)
    {
        return participantCount * basePricePerPerson;
    }

    public decimal CalculateUpgradeFee(decimal originalDailyRate, decimal newDailyRate, DateTime startTime, DateTime endTime)
    {
        var originalPrice = CalculateRentalPrice(startTime, endTime, originalDailyRate);
        var newPrice = CalculateRentalPrice(startTime, endTime, newDailyRate);
        
        return newPrice - originalPrice;
    }
}

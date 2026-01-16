using BLL.Enums;
using BLL.Interfaces;
using FluentValidation;

namespace BLL.Validators;

public class RentalValidator : AbstractValidator<Rental>
{
    private readonly IAvailabilityService _availabilityService;
    
    public RentalValidator(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
        
        RuleFor(r => r.EndDate)
            .GreaterThanOrEqualTo(r => r.StartDate)
            .WithMessage("End date must be on or after start date");
            
        RuleFor(r => r.EndTime)
            .Must((rental, endTime) => 
            {
                if (rental.StartDate == rental.EndDate)
                    return endTime > rental.StartTime;
                return true;
            })
            .WithMessage("End time must be after start time for same-day rentals");
            
        RuleFor(r => r.StartTime)
            .Must(BeValidTimeBlock)
            .When(r => r.RentalType == RentalType.FourHour)
            .WithMessage("4-hour rentals must start at 09:00, 13:00, 17:00, or 21:00");
            
        RuleFor(r => r)
            .MustAsync(async (rental, cancellation) => 
            {
                var startDateTime = rental.StartDate.ToDateTime(rental.StartTime);
                var endDateTime = rental.EndDate.ToDateTime(rental.EndTime);
                return await _availabilityService.IsBikeAvailableAsync(
                    rental.BikeId, startDateTime, endDateTime);
            })
            .WithMessage("Bike is not available for the requested period");
    }
    
    private bool BeValidTimeBlock(TimeOnly time)
    {
        var validTimes = new[] { 
            new TimeOnly(9, 0), 
            new TimeOnly(13, 0), 
            new TimeOnly(17, 0), 
            new TimeOnly(21, 0) 
        };
        return validTimes.Contains(time);
    }
}

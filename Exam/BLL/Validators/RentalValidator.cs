using BLL.Enums;
using FluentValidation;

namespace BLL.Validators;

public class RentalValidator : AbstractValidator<Rental>
{
    public RentalValidator()
    {
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

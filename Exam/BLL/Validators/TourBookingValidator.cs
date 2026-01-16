using BLL.Interfaces;
using FluentValidation;

namespace BLL.Validators;

public class TourBookingValidator : AbstractValidator<TourBooking>
{
    private readonly ITourService _tourService;
    
    public TourBookingValidator(ITourService tourService)
    {
        _tourService = tourService;
        
        RuleFor(tb => tb.ParticipantCount)
            .GreaterThan(0)
            .WithMessage("Must have at least one participant");
            
        RuleFor(tb => tb)
            .MustAsync(async (booking, cancellation) =>
            {
                var available = await _tourService.GetRemainingCapacityAsync(
                    booking.TourId, booking.BookingDate, booking.TimeSlot);
                return booking.ParticipantCount <= available;
            })
            .WithMessage(booking => 
                $"Cannot book {booking.ParticipantCount} participants - exceeds capacity");
    }
}

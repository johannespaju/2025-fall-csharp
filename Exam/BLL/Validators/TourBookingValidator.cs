using BLL.Interfaces;
using FluentValidation;

namespace BLL.Validators;

public class TourBookingValidator : AbstractValidator<TourBooking>
{
    public TourBookingValidator(ITourService tourService)
    {
        RuleFor(tb => tb.ParticipantCount)
            .GreaterThan(0)
            .WithMessage("Must have at least one participant");
    }
}

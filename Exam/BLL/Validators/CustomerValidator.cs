using FluentValidation;

namespace BLL.Validators;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(50);
            
        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(50);
            
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Must be a valid email address");
            
        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?\d{7,15}$")
            .WithMessage("Phone must be 7-15 digits, optionally starting with +");
            
        RuleFor(c => c.DamageIncidentCount)
            .GreaterThanOrEqualTo(0);
    }
}

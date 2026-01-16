using FluentValidation;

namespace BLL.Validators;

public class BikeValidator : AbstractValidator<Bike>
{
    public BikeValidator()
    {
        RuleFor(b => b.BikeNumber)
            .NotEmpty()
            .Matches(@"^[A-Z]+-\d{3}$")
            .WithMessage("Bike number must be in format: TYPE-001");
            
        RuleFor(b => b.Type)
            .IsInEnum();
            
        RuleFor(b => b.DailyRate)
            .GreaterThan(0)
            .WithMessage("Daily rate must be positive");
            
        RuleFor(b => b.CurrentOdometer)
            .GreaterThanOrEqualTo(0);
            
        RuleFor(b => b.ServiceInterval)
            .GreaterThan(0)
            .Must((bike, interval) => interval == GetRequiredInterval(bike.Type))
            .WithMessage(bike => $"{bike.Type} bikes must have {GetRequiredInterval(bike.Type)}km service interval");
    }
    
    private int GetRequiredInterval(BikeType type) => type switch
    {
        BikeType.City => 500,
        BikeType.Electric => 300,
        BikeType.Mountain => 400,
        _ => 500
    };
}

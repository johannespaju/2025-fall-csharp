using BLL.Enums;

namespace BLL;

public class Rental : BaseEntity
{
    // Date/Time separation (Requirements.MD line 94-95)
    public DateOnly StartDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly EndTime { get; set; }
    
    // Rental metadata
    public RentalType RentalType { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Reserved;
    
    // Return information
    public DateOnly? ActualReturnDate { get; set; }
    public int? ReturnOdometer { get; set; }
    
    // Financial
    public decimal DepositAmount { get; set; }
    public decimal TotalCost { get; set; }

    // Foreign keys
    public Guid BikeId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? TourBookingId { get; set; } // Nullable if not part of a tour

    // Navigation properties
    public Bike? Bike { get; set; }
    public Customer? Customer { get; set; }
    public TourBooking? TourBooking { get; set; }
}

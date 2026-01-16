namespace BLL;

public class Rental : BaseEntity
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public decimal Deposit { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public Guid BikeId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? TourBookingId { get; set; } // Nullable if not part of a tour

    // Navigation properties
    public Bike Bike { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public TourBooking? TourBooking { get; set; }
}
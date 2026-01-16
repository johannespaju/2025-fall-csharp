using BLL.Enums;

namespace BLL;

public class TourBooking : BaseEntity
{
    public DateOnly BookingDate { get; set; }
    public TimeOnly TimeSlot { get; set; }
    public int ParticipantCount { get; set; }
    public bool BikeUpgradeToElectric { get; set; } = false;
    public decimal TotalCost { get; set; }
    public TourBookingStatus Status { get; set; } = TourBookingStatus.Confirmed;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? SpecialRequests { get; set; }

    // Foreign keys
    public Guid TourId { get; set; }
    public Guid CustomerId { get; set; }

    // Navigation properties
    public Tour Tour { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}

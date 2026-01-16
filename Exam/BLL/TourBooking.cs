namespace BLL;

public class TourBooking : BaseEntity
{
    public int ParticipantCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SpecialRequests { get; set; }

    // Foreign keys
    public Guid TourId { get; set; }
    public Guid CustomerId { get; set; }

    // Navigation properties
    public Tour Tour { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
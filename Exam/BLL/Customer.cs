namespace BLL;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
    public ICollection<DamageRecord> DamageRecords { get; set; } = new List<DamageRecord>();
}
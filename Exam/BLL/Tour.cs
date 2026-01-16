namespace BLL;

public enum TourType
{
    City,
    Coastal,
    Mountain
}

public class Tour : BaseEntity
{
    public TourType Type { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerParticipant { get; set; }

    // Navigation properties
    public ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
}
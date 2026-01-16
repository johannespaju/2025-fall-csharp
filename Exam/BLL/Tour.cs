namespace BLL;

public enum TourType
{
    City,
    Coastal,
    Mountain
}

public class Tour : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TourType Type { get; set; }
    public decimal DurationHours { get; set; }
    public int MaxCapacity { get; set; }
    public BikeType IncludedBikeType { get; set; } = BikeType.City;
    public decimal PricePerParticipant { get; set; }
    public decimal UpgradeToElectricFee { get; set; } = 10.00m;
    public string TimeSlots { get; set; } = "10:00,15:00"; // CSV format
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
}

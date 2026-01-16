namespace BLL;

public enum BikeType
{
    City,
    Electric,
    Mountain,
    Tandem,
    Children
}

public class Bike : BaseEntity
{
    public BikeType Type { get; set; }
    public decimal DailyRate { get; set; }
    public int ServiceInterval { get; set; } // in kilometers
    public int CurrentOdometer { get; set; } // in kilometers
    public bool IsAvailable { get; set; } = true;

    // Calculated property for maintenance status with 50km buffer
    public bool NeedsMaintenance => CurrentOdometer >= (ServiceInterval - 50);

    // Navigation properties
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<DamageRecord> DamageRecords { get; set; } = new List<DamageRecord>();
}
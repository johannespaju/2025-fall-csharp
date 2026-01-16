using BLL.Enums;

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
    public string BikeNumber { get; set; } = default!; // Format: "CITY-001", unique
    public BikeType Type { get; set; }
    public BikeStatus Status { get; set; } = BikeStatus.Available;
    public decimal DailyRate { get; set; }
    public decimal HourlyRate => DailyRate / 2; // Calculated property
    public int ServiceInterval { get; set; } // in kilometers
    public int CurrentOdometer { get; set; } // in kilometers
    public int LastServiceOdometer { get; set; } = 0; // Track service history

    // Calculated property for maintenance status with 50km buffer
    public bool NeedsMaintenance => (CurrentOdometer - LastServiceOdometer) >= (ServiceInterval - 50);

    // Navigation properties
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<DamageRecord> DamageRecords { get; set; } = new List<DamageRecord>();
}

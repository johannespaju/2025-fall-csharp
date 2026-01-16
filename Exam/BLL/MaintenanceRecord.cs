namespace BLL;

public class MaintenanceRecord : BaseEntity
{
    public DateTime Date { get; set; }
    public decimal Cost { get; set; }
    public int OdometerAtService { get; set; } // in kilometers
    public string? Description { get; set; }

    // Foreign key
    public Guid BikeId { get; set; }

    // Navigation property
    public Bike Bike { get; set; } = null!;
}
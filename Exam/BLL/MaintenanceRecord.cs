using BLL.Enums;

namespace BLL;

public class MaintenanceRecord : BaseEntity
{
    public ServiceType ServiceType { get; set; } = ServiceType.Regular;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedAt { get; set; } // Nullable - may not be completed yet
    public decimal Cost { get; set; }
    public int OdometerAtService { get; set; } // in kilometers
    public string? Description { get; set; }

    // Foreign key
    public Guid BikeId { get; set; }

    // Navigation property
    public Bike Bike { get; set; } = null!;
}

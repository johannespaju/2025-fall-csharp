namespace BLL;

public class DamageRecord : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public DateTime ReportedDate { get; set; }
    public decimal EstimatedCost { get; set; }
    public bool IsRepaired { get; set; } = false;

    // Foreign keys
    public Guid BikeId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RentalId { get; set; }

    // Navigation properties
    public Bike Bike { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public Rental Rental { get; set; } = null!;
}

namespace BLL;

public class DamageRecord : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }

    // Foreign keys
    public Guid BikeId { get; set; }
    public Guid CustomerId { get; set; }

    // Navigation properties
    public Bike Bike { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}
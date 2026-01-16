using BLL;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Bike> Bikes { get; set; } = null!;
    public DbSet<Rental> Rentals { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Tour> Tours { get; set; } = null!;
    public DbSet<TourBooking> TourBookings { get; set; } = null!;
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; } = null!;
    public DbSet<DamageRecord> DamageRecords { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Bike)
            .WithMany(b => b.Rentals)
            .HasForeignKey(r => r.BikeId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CustomerId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.TourBooking)
            .WithMany(tb => tb.Rentals)
            .HasForeignKey(r => r.TourBookingId);

        modelBuilder.Entity<TourBooking>()
            .HasOne(tb => tb.Tour)
            .WithMany(t => t.TourBookings)
            .HasForeignKey(tb => tb.TourId);

        modelBuilder.Entity<TourBooking>()
            .HasOne(tb => tb.Customer)
            .WithMany(c => c.TourBookings)
            .HasForeignKey(tb => tb.CustomerId);

        modelBuilder.Entity<MaintenanceRecord>()
            .HasOne(mr => mr.Bike)
            .WithMany(b => b.MaintenanceRecords)
            .HasForeignKey(mr => mr.BikeId);

        modelBuilder.Entity<DamageRecord>()
            .HasOne(dr => dr.Bike)
            .WithMany(b => b.DamageRecords)
            .HasForeignKey(dr => dr.BikeId);

        modelBuilder.Entity<DamageRecord>()
            .HasOne(dr => dr.Customer)
            .WithMany(c => c.DamageRecords)
            .HasForeignKey(dr => dr.CustomerId);
    }
}
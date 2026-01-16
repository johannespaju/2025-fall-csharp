using BLL;
using BLL.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Bikes.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Create bikes
        var bikes = new List<Bike>();
        var random = new Random(42); // Fixed seed for reproducible data

        // 20 City bikes (CITY-001 to CITY-020)
        for (int i = 1; i <= 20; i++)
        {
            bikes.Add(new Bike
            {
                BikeNumber = $"CITY-{i:D3}",
                Type = BikeType.City,
                Status = BikeStatus.Available,
                DailyRate = 12.00m,
                ServiceInterval = 500,
                CurrentOdometer = random.Next(0, 5000),
                LastServiceOdometer = 0
            });
        }

        // 25 Electric bikes (ELECTRIC-001 to ELECTRIC-025)
        for (int i = 1; i <= 25; i++)
        {
            bikes.Add(new Bike
            {
                BikeNumber = $"ELECTRIC-{i:D3}",
                Type = BikeType.Electric,
                Status = BikeStatus.Available,
                DailyRate = 28.00m,
                ServiceInterval = 300,
                CurrentOdometer = random.Next(0, 5000),
                LastServiceOdometer = 0
            });
        }

        // 15 Mountain bikes (MOUNTAIN-001 to MOUNTAIN-015)
        for (int i = 1; i <= 15; i++)
        {
            bikes.Add(new Bike
            {
                BikeNumber = $"MOUNTAIN-{i:D3}",
                Type = BikeType.Mountain,
                Status = BikeStatus.Available,
                DailyRate = 18.00m,
                ServiceInterval = 400,
                CurrentOdometer = random.Next(0, 5000),
                LastServiceOdometer = 0
            });
        }

        // 10 Tandem bikes (TANDEM-001 to TANDEM-010)
        for (int i = 1; i <= 10; i++)
        {
            bikes.Add(new Bike
            {
                BikeNumber = $"TANDEM-{i:D3}",
                Type = BikeType.Tandem,
                Status = BikeStatus.Available,
                DailyRate = 22.00m,
                ServiceInterval = 500,
                CurrentOdometer = random.Next(0, 5000),
                LastServiceOdometer = 0
            });
        }

        // 10 Children's bikes (CHILDREN-001 to CHILDREN-010)
        for (int i = 1; i <= 10; i++)
        {
            bikes.Add(new Bike
            {
                BikeNumber = $"CHILDREN-{i:D3}",
                Type = BikeType.Children,
                Status = BikeStatus.Available,
                DailyRate = 8.00m,
                ServiceInterval = 500,
                CurrentOdometer = random.Next(0, 5000),
                LastServiceOdometer = 0
            });
        }

        await context.Bikes.AddRangeAsync(bikes);

        // Create 10 customers (at least 2 with damage history)
        var customers = new List<Customer>
        {
            new Customer
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@example.com",
                PhoneNumber = "+372 5123 4567",
                DamageIncidentCount = 0
            },
            new Customer
            {
                FirstName = "Emma",
                LastName = "Johnson",
                Email = "emma.johnson@example.com",
                PhoneNumber = "+372 5234 5678",
                DamageIncidentCount = 2 // Has damage history
            },
            new Customer
            {
                FirstName = "Michael",
                LastName = "Williams",
                Email = "michael.williams@example.com",
                PhoneNumber = "+372 5345 6789",
                DamageIncidentCount = 0
            },
            new Customer
            {
                FirstName = "Sarah",
                LastName = "Brown",
                Email = "sarah.brown@example.com",
                PhoneNumber = "+372 5456 7890",
                DamageIncidentCount = 1 // Has damage history
            },
            new Customer
            {
                FirstName = "David",
                LastName = "Jones",
                Email = "david.jones@example.com",
                PhoneNumber = "+372 5567 8901",
                DamageIncidentCount = 0
            },
            new Customer
            {
                FirstName = "Lisa",
                LastName = "Garcia",
                Email = "lisa.garcia@example.com",
                PhoneNumber = "+372 5678 9012",
                DamageIncidentCount = 0
            },
            new Customer
            {
                FirstName = "James",
                LastName = "Miller",
                Email = "james.miller@example.com",
                PhoneNumber = "+372 5789 0123",
                DamageIncidentCount = 0
            },
            new Customer
            {
                FirstName = "Anna",
                LastName = "Davis",
                Email = "anna.davis@example.com",
                PhoneNumber = "+372 5890 1234",
                DamageIncidentCount = 0
            },
            new Customer
            {
                FirstName = "Robert",
                LastName = "Martinez",
                Email = "robert.martinez@example.com",
                PhoneNumber = "+372 5901 2345",
                DamageIncidentCount = 0
            },
            new Customer
            {
                FirstName = "Maria",
                LastName = "Rodriguez",
                Email = "maria.rodriguez@example.com",
                PhoneNumber = "+372 5012 3456",
                DamageIncidentCount = 0
            }
        };

        await context.Customers.AddRangeAsync(customers);

        // Create 3 tour templates
        var tours = new List<Tour>
        {
            new Tour
            {
                Name = "City Tour",
                Description = "Explore the historic city center and main attractions on a guided bicycle tour.",
                Type = TourType.City,
                DurationHours = 3.0m,
                MaxCapacity = 12,
                IncludedBikeType = BikeType.City,
                PricePerParticipant = 35.00m,
                UpgradeToElectricFee = 10.00m,
                TimeSlots = "10:00,14:00,17:00",
                IsActive = true
            },
            new Tour
            {
                Name = "Coastal Tour",
                Description = "Scenic coastal route with beautiful sea views and beach stops.",
                Type = TourType.Coastal,
                DurationHours = 5.0m,
                MaxCapacity = 8,
                IncludedBikeType = BikeType.City,
                PricePerParticipant = 55.00m,
                UpgradeToElectricFee = 10.00m,
                TimeSlots = "09:00,13:00",
                IsActive = true
            },
            new Tour
            {
                Name = "Mountain Tour",
                Description = "Challenging mountain trails for experienced riders with stunning panoramic views.",
                Type = TourType.Mountain,
                DurationHours = 6.0m,
                MaxCapacity = 6,
                IncludedBikeType = BikeType.Mountain,
                PricePerParticipant = 75.00m,
                UpgradeToElectricFee = 15.00m,
                TimeSlots = "08:00,12:00",
                IsActive = true
            }
        };

        await context.Tours.AddRangeAsync(tours);

        // Save all changes
        await context.SaveChangesAsync();
    }
}

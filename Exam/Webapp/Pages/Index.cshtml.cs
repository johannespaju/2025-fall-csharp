using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages;

public class IndexModel : PageModel
{
    private readonly IRepository<Bike> _bikeRepo;
    private readonly IRepository<Rental> _rentalRepo;
    private readonly IRepository<Customer> _customerRepo;
    private readonly IRepository<Tour> _tourRepo;
    private readonly IMaintenanceService _maintenanceService;
    
    public int TotalBikes { get; set; }
    public int AvailableBikes { get; set; }
    public int RentedBikes { get; set; }
    public int MaintenanceBikes { get; set; }
    public int DamagedBikes { get; set; }
    
    public int ActiveRentals { get; set; }
    public int FutureRentals { get; set; }
    public int CompletedRentals { get; set; }
    
    public int TotalCustomers { get; set; }
    public int TotalTours { get; set; }
    
    public List<Bike> BikesNeedingMaintenance { get; set; } = new();
    public List<Rental> RecentRentals { get; set; } = new();
    
    public IndexModel(
        IRepository<Bike> bikeRepo,
        IRepository<Rental> rentalRepo,
        IRepository<Customer> customerRepo,
        IRepository<Tour> tourRepo,
        IMaintenanceService maintenanceService)
    {
        _bikeRepo = bikeRepo;
        _rentalRepo = rentalRepo;
        _customerRepo = customerRepo;
        _tourRepo = tourRepo;
        _maintenanceService = maintenanceService;
    }
    
    public async Task OnGetAsync()
    {
        // Bike statistics
        var bikes = await _bikeRepo.GetAllAsync();
        TotalBikes = bikes.Count();
        AvailableBikes = bikes.Count(b => b.Status == BikeStatus.Available);
        RentedBikes = bikes.Count(b => b.Status == BikeStatus.Rented);
        MaintenanceBikes = bikes.Count(b => b.Status == BikeStatus.InMaintenance);
        DamagedBikes = bikes.Count(b => b.Status == BikeStatus.Damaged);
        
        // Rental statistics
        var rentals = await _rentalRepo.GetAllAsync();
        var today = DateOnly.FromDateTime(DateTime.Today);
        ActiveRentals = rentals.Count(r => r.Status == RentalStatus.Active);
        FutureRentals = rentals.Count(r => 
            r.Status == RentalStatus.Reserved && r.StartDate > today);
        CompletedRentals = rentals.Count(r => r.Status == RentalStatus.Completed);
        
        // Other counts
        var customers = await _customerRepo.GetAllAsync();
        TotalCustomers = customers.Count();
        var tours = await _tourRepo.GetAllAsync();
        TotalTours = tours.Count();
        
        // Bikes needing maintenance
        foreach (var bike in bikes)
        {
            if (await _maintenanceService.ShouldFlagForMaintenanceAsync(bike.Id))
                BikesNeedingMaintenance.Add(bike);
        }
        
        // Recent rentals (last 10)
        RecentRentals = rentals
            .OrderByDescending(r => r.StartDate)
            .ThenByDescending(r => r.StartTime)
            .Take(10)
            .ToList();
    }
}

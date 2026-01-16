using BLL;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Rentals;

public class ReturnModel : PageModel
{
    private readonly IRepository<Rental> _rentalRepository;
    private readonly IRepository<Bike> _bikeRepository;
    private readonly IRepository<DamageRecord> _damageRecordRepository;
    private readonly IMaintenanceService _maintenanceService;

    public ReturnModel(
        IRepository<Rental> rentalRepository,
        IRepository<Bike> bikeRepository,
        IRepository<DamageRecord> damageRecordRepository,
        IMaintenanceService maintenanceService)
    {
        _rentalRepository = rentalRepository;
        _bikeRepository = bikeRepository;
        _damageRecordRepository = damageRecordRepository;
        _maintenanceService = maintenanceService;
    }

    public Rental Rental { get; set; } = new();

    [BindProperty]
    public int OdometerReading { get; set; }

    [BindProperty]
    public bool HasDamage { get; set; }

    [BindProperty]
    public string? DamageDescription { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Rental = await _rentalRepository.GetByIdAsync(id);
        if (Rental == null)
        {
            return NotFound();
        }

        OdometerReading = Rental.Bike.CurrentOdometer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Update bike odometer
        var bike = await _bikeRepository.GetByIdAsync(Rental.BikeId);
        bike!.CurrentOdometer = OdometerReading;
        bike.IsAvailable = true; // Bike becomes available again
        await _bikeRepository.UpdateAsync(bike);

        // Create damage record if needed
        if (HasDamage)
        {
            var damageRecord = new DamageRecord
            {
                BikeId = Rental.BikeId,
                CustomerId = Rental.CustomerId,
                Date = DateTime.Now,
                Description = DamageDescription
            };
            await _damageRecordRepository.AddAsync(damageRecord);
        }

        // Check if maintenance is needed
        if (await _maintenanceService.IsBikeDueForMaintenanceAsync(Rental.BikeId))
        {
            bike.IsAvailable = false; // Flag as unavailable for maintenance
            await _maintenanceService.FlagBikeForMaintenanceAsync(Rental.BikeId);
            await _bikeRepository.UpdateAsync(bike);
        }

        // Mark rental as completed
        Rental.IsActive = false;
        await _rentalRepository.UpdateAsync(Rental);
        await _rentalRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }
}

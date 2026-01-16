using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Webapp.Pages.Maintenance;

public class CreateModel : PageModel
{
    private readonly IRepository<MaintenanceRecord> _maintenanceRepository;
    private readonly IRepository<Bike> _bikeRepository;

    public CreateModel(
        IRepository<MaintenanceRecord> maintenanceRepository,
        IRepository<Bike> bikeRepository)
    {
        _maintenanceRepository = maintenanceRepository;
        _bikeRepository = bikeRepository;
    }

    [BindProperty]
    public MaintenanceRecord MaintenanceRecord { get; set; } = new();

    [BindProperty]
    public bool MarkAsCompleted { get; set; } = true;

    public SelectList BikeSelectList { get; set; } = null!;
    public Bike? SelectedBike { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? bikeId)
    {
        await LoadBikesAsync();

        MaintenanceRecord.ScheduledDate = DateTime.Today;

        if (bikeId.HasValue)
        {
            var bike = await _bikeRepository.GetByIdAsync(bikeId.Value);
            if (bike != null)
            {
                SelectedBike = bike;
                MaintenanceRecord.BikeId = bike.Id;
                MaintenanceRecord.OdometerAtService = bike.CurrentOdometer;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadBikesAsync();

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return Page();
        }

        var bike = await _bikeRepository.GetByIdAsync(MaintenanceRecord.BikeId);
        if (bike == null)
        {
            TempData["ErrorMessage"] = "Selected bike not found.";
            return Page();
        }

        if (MarkAsCompleted)
        {
            MaintenanceRecord.CompletedAt = DateTime.UtcNow;
            bike.Status = BikeStatus.Available;
            bike.LastServiceOdometer = MaintenanceRecord.OdometerAtService;
            bike.ServiceCosts += MaintenanceRecord.Cost;
        }

        await _maintenanceRepository.AddAsync(MaintenanceRecord);
        await _maintenanceRepository.SaveChangesAsync();

        await _bikeRepository.UpdateAsync(bike);
        await _bikeRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Maintenance recorded for bike {bike.BikeNumber}!";
        return RedirectToPage(nameof(Index));
    }

    private async Task LoadBikesAsync()
    {
        var bikes = await _bikeRepository.GetAllAsync();
        BikeSelectList = new SelectList(
            bikes.OrderBy(b => b.BikeNumber),
            nameof(Bike.Id),
            nameof(Bike.BikeNumber));
    }
}

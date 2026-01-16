using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Webapp.Pages.Maintenance;

public class EditModel : PageModel
{
    private readonly IRepository<MaintenanceRecord> _maintenanceRepository;
    private readonly IRepository<Bike> _bikeRepository;

    public EditModel(
        IRepository<MaintenanceRecord> maintenanceRepository,
        IRepository<Bike> bikeRepository)
    {
        _maintenanceRepository = maintenanceRepository;
        _bikeRepository = bikeRepository;
    }

    [BindProperty]
    public MaintenanceRecord MaintenanceRecord { get; set; } = new();

    [BindProperty]
    public bool MarkAsCompleted { get; set; }

    public SelectList BikeSelectList { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var record = await _maintenanceRepository.GetByIdAsync(id);
        if (record == null)
        {
            return NotFound();
        }

        MaintenanceRecord = record;
        await LoadBikesAsync();

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

        var existingRecord = await _maintenanceRepository.GetByIdAsync(MaintenanceRecord.Id);
        if (existingRecord == null)
        {
            return NotFound();
        }

        // Update fields
        existingRecord.BikeId = MaintenanceRecord.BikeId;
        existingRecord.ServiceType = MaintenanceRecord.ServiceType;
        existingRecord.ScheduledDate = MaintenanceRecord.ScheduledDate;
        existingRecord.OdometerAtService = MaintenanceRecord.OdometerAtService;
        existingRecord.Cost = MaintenanceRecord.Cost;
        existingRecord.Description = MaintenanceRecord.Description;

        // Handle completion
        if (MarkAsCompleted && !existingRecord.CompletedAt.HasValue)
        {
            existingRecord.CompletedAt = DateTime.UtcNow;

            // Update bike status
            var bike = await _bikeRepository.GetByIdAsync(MaintenanceRecord.BikeId);
            if (bike != null)
            {
                bike.Status = BikeStatus.Available;
                bike.LastServiceOdometer = MaintenanceRecord.OdometerAtService;
                await _bikeRepository.UpdateAsync(bike);
                await _bikeRepository.SaveChangesAsync();
            }
        }

        await _maintenanceRepository.UpdateAsync(existingRecord);
        await _maintenanceRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = "Maintenance record updated successfully!";
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

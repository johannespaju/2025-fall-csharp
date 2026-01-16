using BLL;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Maintenance;

public class DeleteModel : PageModel
{
    private readonly IRepository<MaintenanceRecord> _maintenanceRepository;
    private readonly IRepository<Bike> _bikeRepository;

    public DeleteModel(
        IRepository<MaintenanceRecord> maintenanceRepository,
        IRepository<Bike> bikeRepository)
    {
        _maintenanceRepository = maintenanceRepository;
        _bikeRepository = bikeRepository;
    }

    [BindProperty]
    public MaintenanceRecord MaintenanceRecord { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var records = await _maintenanceRepository.GetAllAsync("Bike");
        var record = records.FirstOrDefault(r => r.Id == id);
        if (record == null)
        {
            return NotFound();
        }

        MaintenanceRecord = record;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var record = await _maintenanceRepository.GetByIdAsync(MaintenanceRecord.Id);
        if (record == null)
        {
            return NotFound();
        }

        await _maintenanceRepository.DeleteAsync(record);
        await _maintenanceRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = "Maintenance record deleted successfully!";
        return RedirectToPage(nameof(Index));
    }
}

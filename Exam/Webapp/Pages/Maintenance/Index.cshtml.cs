using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Maintenance;

public class IndexModel : PageModel
{
    private readonly IRepository<MaintenanceRecord> _maintenanceRepository;
    private readonly IRepository<Bike> _bikeRepository;
    private readonly IMaintenanceService _maintenanceService;

    public IndexModel(
        IRepository<MaintenanceRecord> maintenanceRepository,
        IRepository<Bike> bikeRepository,
        IMaintenanceService maintenanceService)
    {
        _maintenanceRepository = maintenanceRepository;
        _bikeRepository = bikeRepository;
        _maintenanceService = maintenanceService;
    }

    public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new();
    public List<Bike> BikesNeedingMaintenance { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public ServiceType? FilterServiceType { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? FilterCompleted { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        var records = await _maintenanceRepository.GetAllAsync("Bike");
        var query = records.AsQueryable();

        if (FilterServiceType.HasValue)
        {
            query = query.Where(r => r.ServiceType == FilterServiceType.Value);
        }

        if (FilterCompleted.HasValue)
        {
            query = FilterCompleted.Value
                ? query.Where(r => r.CompletedAt.HasValue)
                : query.Where(r => !r.CompletedAt.HasValue);
        }

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(r => r.Bike != null &&
                r.Bike.BikeNumber.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        MaintenanceRecords = query
            .OrderByDescending(r => r.ScheduledDate)
            .ToList();

        // Get bikes needing maintenance
        var bikes = await _bikeRepository.GetAllAsync();
        foreach (var bike in bikes)
        {
            if (await _maintenanceService.ShouldFlagForMaintenanceAsync(bike.Id) &&
                bike.Status != BikeStatus.InMaintenance)
            {
                BikesNeedingMaintenance.Add(bike);
            }
        }

        // Also include bikes already in maintenance status
        BikesNeedingMaintenance.AddRange(
            bikes.Where(b => b.Status == BikeStatus.InMaintenance &&
                !BikesNeedingMaintenance.Any(m => m.Id == b.Id)));
    }
}

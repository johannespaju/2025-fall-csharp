using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Bikes;

public class IndexModel : PageModel
{
    private readonly IRepository<Bike> _bikeRepository;
    private readonly IMaintenanceService _maintenanceService;

    public IndexModel(IRepository<Bike> bikeRepository, IMaintenanceService maintenanceService)
    {
        _bikeRepository = bikeRepository;
        _maintenanceService = maintenanceService;
    }

    public List<Bike> Bikes { get; set; } = new();
    public List<Guid> MaintenanceDueBikes { get; set; } = new();

    public async Task OnGetAsync()
    {
        Bikes = (await _bikeRepository.GetAllAsync()).ToList();
        MaintenanceDueBikes = (await _maintenanceService.GetBikesDueForMaintenanceAsync()).ToList();
    }
}

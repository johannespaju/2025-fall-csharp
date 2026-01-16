using BLL;
using BLL.Enums;
using BLL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Bikes;

public class IndexModel : PageModel
{
    private readonly IBikeRepository _bikeRepository;
    private readonly IMaintenanceService _maintenanceService;

    public IndexModel(IBikeRepository bikeRepository, IMaintenanceService maintenanceService)
    {
        _bikeRepository = bikeRepository;
        _maintenanceService = maintenanceService;
    }

    public List<Bike> Bikes { get; set; } = new();
    public List<Guid> MaintenanceDueBikes { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public BikeType? FilterType { get; set; }

    [BindProperty(SupportsGet = true)]
    public BikeStatus? FilterStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        Bikes = (await _bikeRepository.SearchBikesAsync(FilterType, FilterStatus, SearchTerm)).ToList();
        MaintenanceDueBikes = (await _maintenanceService.GetBikesDueForMaintenanceAsync()).ToList();
    }
}

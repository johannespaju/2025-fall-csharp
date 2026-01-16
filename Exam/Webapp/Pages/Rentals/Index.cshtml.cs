using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Rentals;

public class IndexModel : PageModel
{
    private readonly IRepository<Rental> _rentalRepository;

    public IndexModel(IRepository<Rental> rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public List<Rental> Rentals { get; set; } = new();

    public async Task OnGetAsync()
    {
        Rentals = (await _rentalRepository.GetAllAsync("Bike", "Customer")).ToList();
    }
}

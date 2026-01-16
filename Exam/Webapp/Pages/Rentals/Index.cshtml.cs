using BLL;
using BLL.Enums;
using BLL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Rentals;

public class IndexModel : PageModel
{
    private readonly IRentalRepository _rentalRepository;

    public IndexModel(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public List<Rental> Rentals { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public RentalStatus? FilterStatus { get; set; }

    public async Task OnGetAsync()
    {
        Rentals = (await _rentalRepository.SearchRentalsAsync(FilterStatus)).ToList();
    }
}

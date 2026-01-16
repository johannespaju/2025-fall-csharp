using BLL;
using BLL.Enums;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Reservations;

public class IndexModel : PageModel
{
    private readonly IRentalRepository _rentalRepository;

    public IndexModel(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public List<Rental> Reservations { get; set; } = new();

    public async Task OnGetAsync()
    {
        Reservations = (await _rentalRepository.SearchRentalsAsync(RentalStatus.Reserved)).ToList();
    }
}

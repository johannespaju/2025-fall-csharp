using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Tours;

public class DeleteModel : PageModel
{
    private readonly IRepository<Tour> _tourRepository;
    private readonly IRepository<TourBooking> _tourBookingRepository;

    public DeleteModel(
        IRepository<Tour> tourRepository,
        IRepository<TourBooking> tourBookingRepository)
    {
        _tourRepository = tourRepository;
        _tourBookingRepository = tourBookingRepository;
    }

    [BindProperty]
    public Tour Tour { get; set; } = new();

    public bool HasBookings { get; set; }
    public int TotalParticipants { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Tour = await _tourRepository.GetByIdAsync(id);
        if (Tour == null)
        {
            return NotFound();
        }

        var bookings = await _tourBookingRepository.GetAllAsync();
        var tourBookings = bookings.Where(b => b.TourId == id);
        HasBookings = tourBookings.Any();
        TotalParticipants = tourBookings.Sum(b => b.ParticipantCount);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _tourRepository.DeleteAsync(Tour);
        await _tourRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }
}

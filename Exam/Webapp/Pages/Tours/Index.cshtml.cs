using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Tours;

public class IndexModel : PageModel
{
    private readonly IRepository<Tour> _tourRepository;
    private readonly IRepository<TourBooking> _tourBookingRepository;

    public IndexModel(
        IRepository<Tour> tourRepository,
        IRepository<TourBooking> tourBookingRepository)
    {
        _tourRepository = tourRepository;
        _tourBookingRepository = tourBookingRepository;
    }

    public List<(Tour Tour, int BookingCount)> ToursWithCounts { get; set; } = new();

    public async Task OnGetAsync()
    {
        var tours = (await _tourRepository.GetAllAsync()).ToList();
        var bookings = await _tourBookingRepository.GetAllAsync();
        var bookingCounts = bookings.GroupBy(b => b.TourId)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.ParticipantCount));

        ToursWithCounts = tours.Select(t => (t, bookingCounts.TryGetValue(t.Id, out var count) ? count : 0)).ToList();
    }
}

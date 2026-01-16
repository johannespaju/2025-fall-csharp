using BLL;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Rentals;

public class ExtendModel : PageModel
{
    private readonly IRepository<Rental> _rentalRepository;
    private readonly IRepository<Bike> _bikeRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly IPricingService _pricingService;

    public ExtendModel(
        IRepository<Rental> rentalRepository,
        IRepository<Bike> bikeRepository,
        IAvailabilityService availabilityService,
        IPricingService pricingService)
    {
        _rentalRepository = rentalRepository;
        _bikeRepository = bikeRepository;
        _availabilityService = availabilityService;
        _pricingService = pricingService;
    }

    public Rental Rental { get; set; } = new();

    [BindProperty]
    public DateTime NewEndTime { get; set; }

    public decimal AdditionalCost { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Rental = await _rentalRepository.GetByIdAsync(id);
        if (Rental == null)
        {
            return NotFound();
        }

        NewEndTime = Rental.EndTime.AddHours(4); // Default: extend by 4 hours
        CalculateAdditionalCost();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Check if extension is possible
        var isExtendable = await _availabilityService.IsRentalExtendableAsync(Rental.Id, NewEndTime);
        if (!isExtendable)
        {
            ModelState.AddModelError(string.Empty, "Cannot extend rental - bike is not available for the requested time.");
            return Page();
        }

        // Calculate additional cost
        var bike = await _bikeRepository.GetByIdAsync(Rental.BikeId);
        var totalPrice = _pricingService.CalculateRentalPrice(
            Rental.StartTime, NewEndTime, bike!.DailyRate);
        var additionalCost = totalPrice - Rental.TotalPrice;

        // Update rental
        Rental.EndTime = NewEndTime;
        Rental.TotalPrice = totalPrice;
        await _rentalRepository.UpdateAsync(Rental);
        await _rentalRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }

    private void CalculateAdditionalCost()
    {
        // This would typically be calculated based on bike rate and time difference
        AdditionalCost = 0; // Placeholder logic
    }
}

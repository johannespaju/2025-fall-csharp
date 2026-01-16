using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Customers;

public class DeleteModel : PageModel
{
    private readonly IRepository<Customer> _customerRepository;

    public DeleteModel(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [BindProperty]
    public Customer Customer { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        Customer = customer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _customerRepository.DeleteAsync(Customer);
        await _customerRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Customer {Customer.FirstName} {Customer.LastName} deleted successfully!";
        return RedirectToPage(nameof(Index));
    }
}

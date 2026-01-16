using BLL;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Customers;

public class EditModel : PageModel
{
    private readonly IRepository<Customer> _customerRepository;

    public EditModel(IRepository<Customer> customerRepository)
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
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _customerRepository.UpdateAsync(Customer);
        await _customerRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }
}

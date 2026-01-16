using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Customers;

public class CreateModel : PageModel
{
    private readonly IRepository<Customer> _customerRepository;

    public CreateModel(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [BindProperty]
    public Customer Customer { get; set; } = new();

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _customerRepository.AddAsync(Customer);
        await _customerRepository.SaveChangesAsync();

        return RedirectToPage(nameof(Index));
    }
}

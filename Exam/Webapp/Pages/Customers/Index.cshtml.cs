using BLL;
using BLL.Enums;
using BLL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDepositService _depositService;

    public IndexModel(ICustomerRepository customerRepository, IDepositService depositService)
    {
        _customerRepository = customerRepository;
        _depositService = depositService;
    }

    public List<Customer> Customers { get; set; } = new();
    public Dictionary<Guid, int> DamageCounts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        Customers = (await _customerRepository.SearchCustomersAsync(SearchTerm)).ToList();
        
        foreach (var customer in Customers)
        {
            var damageCount = await _depositService.GetCustomerDamageCountAsync(customer.Id);
            DamageCounts.Add(customer.Id, damageCount);
        }
    }
}

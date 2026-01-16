using BLL;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapp.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IDepositService _depositService;

    public IndexModel(IRepository<Customer> customerRepository, IDepositService depositService)
    {
        _customerRepository = customerRepository;
        _depositService = depositService;
    }

    public List<Customer> Customers { get; set; } = new();
    public Dictionary<Guid, int> DamageCounts { get; set; } = new();

    public async Task OnGetAsync()
    {
        Customers = (await _customerRepository.GetAllAsync()).ToList();
        
        foreach (var customer in Customers)
        {
            var damageCount = await _depositService.GetCustomerDamageCountAsync(customer.Id);
            DamageCounts.Add(customer.Id, damageCount);
        }
    }
}

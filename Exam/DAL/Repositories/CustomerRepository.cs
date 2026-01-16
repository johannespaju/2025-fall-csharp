using BLL;
using BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<IEnumerable<Customer>> SearchCustomersAsync(string? searchTerm = null);
}

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }
    
    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email);
    }
    
    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _context.Customers.ToListAsync();
            
        searchTerm = searchTerm.ToLower();
        return await _context.Customers
            .Where(c => c.FirstName.ToLower().Contains(searchTerm)
                     || c.LastName.ToLower().Contains(searchTerm)
                     || c.Email.ToLower().Contains(searchTerm)
                     || c.PhoneNumber.Contains(searchTerm))
            .ToListAsync();
    }
}

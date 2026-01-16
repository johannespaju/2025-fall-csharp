using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetActiveRentalsAsync();
    Task<IEnumerable<Rental>> SearchRentalsAsync(RentalStatus? status = null);
    Task<bool> HasConflictingRentalAsync(
        Guid bikeId, DateTime start, DateTime end, Guid? excludeRentalId = null);
}

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    public RentalRepository(AppDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
    {
        return await _context.Rentals
            .Include(r => r.Bike)
            .Include(r => r.Customer)
            .Where(r => r.Status == RentalStatus.Active)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Rental>> SearchRentalsAsync(RentalStatus? status)
    {
        var query = _context.Rentals
            .Include(r => r.Bike)
            .Include(r => r.Customer)
            .AsQueryable();
            
        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);
            
        return await query.OrderByDescending(r => r.StartDate).ToListAsync();
    }
    
    public async Task<bool> HasConflictingRentalAsync(
        Guid bikeId, DateTime start, DateTime end, Guid? excludeRentalId)
    {
        var query = _context.Rentals
            .Where(r => r.BikeId == bikeId
                     && r.Status != RentalStatus.Cancelled);
                     
        if (excludeRentalId.HasValue)
            query = query.Where(r => r.Id != excludeRentalId.Value);
            
        return await query.AnyAsync(r => 
            r.StartDate.ToDateTime(r.StartTime) < end &&
            r.EndDate.ToDateTime(r.EndTime) > start);
    }
}

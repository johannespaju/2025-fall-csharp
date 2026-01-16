using BLL;
using BLL.Enums;
using BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public interface IBikeRepository : IRepository<Bike>
{
    Task<IEnumerable<Bike>> SearchBikesAsync(
        BikeType? type = null,
        BikeStatus? status = null,
        string? searchTerm = null);
    Task<IEnumerable<Bike>> GetBikesNeedingMaintenanceAsync();
}

public class BikeRepository : Repository<Bike>, IBikeRepository
{
    public BikeRepository(AppDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Bike>> SearchBikesAsync(
        BikeType? type, BikeStatus? status, string? searchTerm)
    {
        var query = _context.Bikes.AsQueryable();
        
        if (type.HasValue)
            query = query.Where(b => b.Type == type.Value);
            
        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);
            
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(b => b.BikeNumber.Contains(searchTerm));
            
        return await query.ToListAsync();
    }
    
    public async Task<IEnumerable<Bike>> GetBikesNeedingMaintenanceAsync()
    {
        const int MAINTENANCE_BUFFER = 50;
        return await _context.Bikes
            .Where(b => (b.CurrentOdometer - b.LastServiceOdometer) >= 
                       (b.ServiceInterval - MAINTENANCE_BUFFER))
            .ToListAsync();
    }
}

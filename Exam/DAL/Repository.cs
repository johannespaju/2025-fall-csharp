using BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class Repository<TData> : IRepository<TData> where TData : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TData?> GetByIdAsync(Guid id)
    {
        return await _context.Set<TData>().FindAsync(id);
    }

    public async Task<IEnumerable<TData>> GetAllAsync(params string[] includes)
    {
        var query = _context.Set<TData>().AsQueryable();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.ToListAsync();
    }

    public async Task AddAsync(TData entity)
    {
        await _context.Set<TData>().AddAsync(entity);
    }

    public async Task UpdateAsync(TData entity)
    {
        _context.Set<TData>().Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(TData entity)
    {
        _context.Set<TData>().Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

namespace BLL.Interfaces;

public interface IRepository<TData> where TData : class
{
    Task<TData?> GetByIdAsync(Guid id);
    Task<IEnumerable<TData>> GetAllAsync(params string[] includes);
    Task AddAsync(TData entity);
    Task UpdateAsync(TData entity);
    Task DeleteAsync(TData entity);
    Task SaveChangesAsync();
}

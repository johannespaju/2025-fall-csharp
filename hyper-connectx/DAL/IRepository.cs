namespace DAL;

public interface IRepository<TData>
{
    
    List<(string id, string description, bool isHidden)> List();
    Task<List<(string id, string description, bool isHidden)>> ListAsync();
    
    string Save(TData data);
    TData Load(string id);
    void Delete(string id);
    
    Task<string> SaveAsync(TData data);
    Task<TData> LoadAsync(string id);
    Task DeleteAsync(string id);
}
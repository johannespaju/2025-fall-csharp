namespace DAL;

public interface IRepository<TData>
{
    
    List<(string id, string description)> List();
    Task<List<(string id, string description)>> ListAsync();
    
    string Save(TData data);
    TData Load(string id);
    void Delete(string id);
}
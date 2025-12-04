namespace DAL;

public interface IRepository<TData>
{
    
    List<string> List();
    
    string Save(TData data);
    TData Load(string id);
    void Delete(string id);
}
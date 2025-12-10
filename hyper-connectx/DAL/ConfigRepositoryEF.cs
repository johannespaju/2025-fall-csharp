using System.Text.RegularExpressions;
using BLL;

namespace DAL;

public class ConfigRepositoryEF : IRepository<GameConfiguration>
{
    private readonly AppDbContext _dbContext;
    
    public ConfigRepositoryEF(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<(string id, string description)> List()
    {
        var res = new List<(string id, string description)>();
        foreach (var dbConf in _dbContext.GameConfigurations)
        {
            res.Add(
                (
                    dbConf.Id.ToString(),
                    $"{dbConf.Name} - {dbConf.BoardWidth}x{dbConf.BoardHeight} - connect{dbConf.ConnectHow}"
                )
            );
        }
        
        return res;
    }

    public string Save(GameConfiguration data)
    {
        var safeName = Regex.Replace(data.Name.Trim(), @"[^a-zA-Z0-9 _\-]", "_");
        data.Name = safeName;
        
        // JSON repo deletes any existing config with the same "safeName" prefix.
        // In EF we’ll treat Name as the logical unique key and upsert on it.
        var existing = _dbContext.GameConfigurations
            .FirstOrDefault(c => c.Name == safeName);
        
        if (existing == null)
        {
            // new config
            _dbContext.GameConfigurations.Add(data);
        }
        else
        {
            // update existing config with values from data (copies all scalar props)
            _dbContext.Entry(existing).CurrentValues.SetValues(data);
        }
        
        _dbContext.SaveChanges();
        
        var entity = existing ?? data;
        return entity.Id.ToString();
    }

    public GameConfiguration Load(string id)
    {
        var conf = _dbContext.GameConfigurations
            .FirstOrDefault(c => c.Id.ToString() == id);

        if (conf == null)
        {
            throw new KeyNotFoundException($"Configuration '{id}' not found.");
        }

        return conf;
    }

    public void Delete(string id)
    {
        var conf = _dbContext.GameConfigurations
            .FirstOrDefault(c => c.Id.ToString() == id);

        if (conf == null)
        {
            // nothing to delete – same behavior as JSON version silently doing nothing
            return;
        }

        _dbContext.GameConfigurations.Remove(conf);
        _dbContext.SaveChanges();
    }
}
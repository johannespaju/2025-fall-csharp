using System.Text.RegularExpressions;
using BLL;
using Microsoft.EntityFrameworkCore;

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
        foreach (var dbConf in _dbContext.GameConfigurations.ToList()) // Added .ToList()
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

    public async Task<List<(string id, string description)>> ListAsync()
    {
        var res = new List<(string id, string description)>();
        foreach (var dbConf in await _dbContext.GameConfigurations.ToListAsync())
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
    
        var existing = _dbContext.GameConfigurations
            .FirstOrDefault(c => c.Name == safeName);
    
        if (existing == null)
        {
            // new config
            _dbContext.GameConfigurations.Add(data);
            _dbContext.SaveChanges();
            return data.Id.ToString();
        }
        else
        {
            // update existing config - manually copy properties except Id
            existing.Name = data.Name;
            existing.BoardWidth = data.BoardWidth;
            existing.BoardHeight = data.BoardHeight;
            existing.ConnectHow = data.ConnectHow;
            // ... copy any other properties you have
        
            _dbContext.SaveChanges();
            return existing.Id.ToString();
        }
    }

    public GameConfiguration Load(string id)
    {
        Console.WriteLine($"DEBUG: Trying to load ID: '{id}'");
    
        if (!Guid.TryParse(id, out var guidId))
        {
            throw new ArgumentException($"Invalid ID format: '{id}'");
        }
    
        var conf = _dbContext.GameConfigurations
            .FirstOrDefault(c => c.Id == guidId);  // Compare Guid to Guid

        if (conf == null)
        {
            throw new KeyNotFoundException($"Configuration '{id}' not found.");
        }

        return conf;
    }

    public void Delete(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
        {
            return; // Invalid ID, nothing to delete
        }
    
        var conf = _dbContext.GameConfigurations
            .FirstOrDefault(c => c.Id == guidId);  // Compare Guid to Guid

        if (conf == null)
        {
            return;
        }

        _dbContext.GameConfigurations.Remove(conf);
        _dbContext.SaveChanges();
    }
}
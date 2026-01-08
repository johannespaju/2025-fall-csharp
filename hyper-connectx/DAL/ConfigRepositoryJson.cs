using System.Text.Json;
using System.Text.RegularExpressions;
using BLL;

namespace DAL;

public class ConfigRepositoryJson : IRepository<GameConfiguration>
{
    public List<(string id, string description, bool isHidden)> List()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        var result = new List<(string id, string description, bool isHidden)>();

        foreach (var fullFileName in Directory.EnumerateFiles(dir, "*.json"))
        {  
            var id = Path.GetFileNameWithoutExtension(fullFileName);
            // Skip non-GUID files for backward compatibility
            if (!Guid.TryParse(id, out _)) continue;
            
            try
            {
                var config = Load(id);
                result.Add(
                    (
                        config.Id.ToString(),
                        $"{config.Name} - {config.BoardWidth}x{config.BoardHeight} - connect{config.ConnectHow}",
                        config.IsHidden
                    )
                );
            }
            catch
            {
                // Skip files that can't be loaded (corrupted or old format)
            }
        }

        return result;
    }
    
    public async Task<List<(string id, string description, bool isHidden)>> ListAsync()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
    
        // Run file enumeration on thread pool to avoid blocking
        return await Task.Run(() =>
        {
            var result = new List<(string id, string description, bool isHidden)>();

            foreach (var fullFileName in Directory.EnumerateFiles(dir, "*.json"))
            {  
                var id = Path.GetFileNameWithoutExtension(fullFileName);
                // Skip non-GUID files for backward compatibility
                if (!Guid.TryParse(id, out _)) continue;
            
                try
                {
                    var config = Load(id);
                    result.Add(
                        (
                            config.Id.ToString(),
                            $"{config.Name} - {config.BoardWidth}x{config.BoardHeight} - connect{config.ConnectHow}",
                            config.IsHidden
                        )
                    );
                }
                catch
                {
                    // Skip files that can't be loaded (corrupted or old format)
                }
            }

            return result;
        });
    }
    
    public string Save(GameConfiguration data)
    {
        // Ensure the configuration has an ID
        if (data.Id == Guid.Empty)
        {
            data.Id = Guid.NewGuid();
        }
        
        var jsonStr = JsonSerializer.Serialize(data);
        
        // Use GUID as filename for consistent lookup
        var fileName = $"{data.Id}.json";
        var dir = FilesystemHelpers.GetConfigDirectory();
        var fullFileName = Path.Combine(dir, fileName);
        
        File.WriteAllText(fullFileName, jsonStr);

        return data.Id.ToString();
    }

    public GameConfiguration Load(string id)
    {
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + id + ".json";
        
        if (!File.Exists(jsonFileName))
            throw new FileNotFoundException($"Configuration '{id}' not found.");
        
        var jsonText = File.ReadAllText(jsonFileName);
        var conf = JsonSerializer.Deserialize<GameConfiguration>(jsonText);

        return conf ?? throw new NullReferenceException("Json deserialization returned null. Data: " + jsonText);
    }

    public void Delete(string id)
    {
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + id + ".json";
        if (File.Exists(jsonFileName))
        {
            File.Delete(jsonFileName);
        }
    }
    // Async methods
    public async Task<string> SaveAsync(GameConfiguration data)
    {
        // Offload to thread pool to avoid blocking
        return await Task.Run(() => Save(data));
    }

    public async Task<GameConfiguration> LoadAsync(string id)
    {
        return await Task.Run(() => Load(id));
    }

    public async Task DeleteAsync(string id)
    {
        await Task.Run(() => Delete(id));
    }
}
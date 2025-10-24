using System.Text.Json;
using System.Text.RegularExpressions;
using BLL;

namespace DLL;

public class ConfigRepositoryJson : IRepository<GameConfiguration>
{
    public List<string> List()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        var result = new List<string>();

        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {  
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json")) continue;
            result.Add(Path.GetFileNameWithoutExtension(fileName));
        }

        return result;
    }
    
    public string Save(GameConfiguration data)
    {
        var jsonStr = JsonSerializer.Serialize(data);
        
        var safeName = Regex.Replace(data.Name.Trim(), @"[^a-zA-Z0-9 _\-]", "_");
        
        var fileName = $"{safeName} - {data.BoardWidth}x{data.BoardHeight} - connect{data.ConnectHow}" + ".json";
        var dir = FilesystemHelpers.GetConfigDirectory();
        var fullFileName = Path.Combine(dir, fileName);
        
        foreach (var existingFile in Directory.EnumerateFiles(dir, "*.json"))
        {
            if (Path.GetFileNameWithoutExtension(existingFile).StartsWith(safeName + " -"))
            {
                File.Delete(existingFile);
                break;
            }
        }
        
        File.WriteAllText(fullFileName, jsonStr);

        return fileName;
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
}
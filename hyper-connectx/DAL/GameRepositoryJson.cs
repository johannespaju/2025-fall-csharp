using System.Text.Json;
using System.Text.RegularExpressions;
using BLL;

namespace DAL;


public class GameRepositoryJson : IRepository<GameState>
{
    public List<(string id, string description)> List()
    {
        var dir = FilesystemHelpers.GetGameDirectory();
        var result = new List<(string id, string description)>();

        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {  
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json")) continue;
            result.Add(
                (
                    Path.GetFileNameWithoutExtension(fileName),
                    Path.GetFileNameWithoutExtension(fileName))
            );
        }

        return result;
    }
    
    public async Task<List<(string id, string description)>> ListAsync()
    {
        var dir = FilesystemHelpers.GetGameDirectory();
    
        return await Task.Run(() =>
        {
            var result = new List<(string id, string description)>();

            foreach (var fullFileName in Directory.EnumerateFiles(dir))
            {  
                var fileName = Path.GetFileName(fullFileName);
                if (!fileName.EndsWith(".json")) continue;
            
                result.Add(
                    (
                        Path.GetFileNameWithoutExtension(fileName),
                        Path.GetFileNameWithoutExtension(fileName)
                    )
                );
            }

            return result;
        });
    }

    public string Save(GameState data)
    {
        var dir = FilesystemHelpers.GetGameDirectory();
        var safeName = Regex.Replace(data.SaveName.Trim(), @"[^a-zA-Z0-9 _\-]", "_");
        var fileName = $"{safeName}.json";
        var fullPath = Path.Combine(dir, fileName);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fullPath, json);
        return Path.GetFileNameWithoutExtension(fileName);
    }

    public GameState Load(string id)
    {
        var path = Path.Combine(FilesystemHelpers.GetGameDirectory(), id + ".json");
        var json = File.ReadAllText(path);
        var state = JsonSerializer.Deserialize<GameState>(json);
        return state ?? throw new Exception("Failed to deserialize game state");
    }

    // Async methods
    public async Task<string> SaveAsync(GameState data)
    {
        // Offload file I/O to a background thread
        return await Task.Run(() => Save(data));
    }

    public async Task<GameState> LoadAsync(string id)
    {
        // Offload file I/O to a background thread
        return await Task.Run(() => Load(id));
    }

    public async Task DeleteAsync(string id)
    {
        // Offload file I/O to a background thread
        await Task.Run(() => Delete(id));
    }

    public void Delete(string id)
    {
        var path = Path.Combine(FilesystemHelpers.GetGameDirectory(), id + ".json");
        if (File.Exists(path)) File.Delete(path);
    }
}
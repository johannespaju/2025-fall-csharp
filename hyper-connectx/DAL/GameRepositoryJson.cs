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
            var id = Path.GetFileNameWithoutExtension(fileName);
            if (!Guid.TryParse(id, out _)) continue; // Only load Guid-named files
            try
            {
                var game = Load(id);
                result.Add((id, game.SaveName));
            }
            catch
            {
                // Skip corrupted files
            }
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
                var id = Path.GetFileNameWithoutExtension(fileName);
                if (!Guid.TryParse(id, out _)) continue;
                try
                {
                    var game = Load(id);
                    result.Add((id, game.SaveName));
                }
                catch
                {
                    Console.WriteLine($"DEBUG: Skipping corrupted file: '{fileName}'");
                    // Skip corrupted files
                }
            }

            return result;
        });
    }

    public string Save(GameState data)
    {
        var dir = FilesystemHelpers.GetGameDirectory();
        var safeName = Regex.Replace(data.SaveName.Trim(), @"[^a-zA-Z0-9 _\-]", "_");
        data.SaveName = safeName; // Update the data with safe name
        var fileName = $"{data.Id}.json";
        var fullPath = Path.Combine(dir, fileName);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fullPath, json);
        return data.Id.ToString();
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
        Console.WriteLine($"DEBUG: Deleting game with id: '{id}'");
        var path = Path.Combine(FilesystemHelpers.GetGameDirectory(), id + ".json");
        Console.WriteLine($"DEBUG: Path: '{path}', exists: {File.Exists(path)}");
        if (File.Exists(path)) File.Delete(path);
        else Console.WriteLine("DEBUG: File does not exist, nothing to delete");
    }
}
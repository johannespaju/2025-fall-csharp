using System.Text.Json;
using BLL;

namespace DAL;


public class GameRepositoryJson : IRepository<GameState>
{
    public List<string> List()
    {
        var dir = FilesystemHelpers.GetGameDirectory();
        return Directory.EnumerateFiles(dir, "*.json")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToList();
    }

    public string Save(GameState data)
    {
        var dir = FilesystemHelpers.GetGameDirectory();
        var safeName = data.SaveName.Replace(":", "_").Replace("/", "_").Replace("\\", "_");
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

    public void Delete(string id)
    {
        var path = Path.Combine(FilesystemHelpers.GetGameDirectory(), id + ".json");
        if (File.Exists(path)) File.Delete(path);
    }
}
using BLL;
using ConsoleApp;
using DLL;
using MenuSystem;

var repo = new ConfigRepositoryJson();
var config = new GameConfiguration();

Console.WriteLine("Hello, ConnectX!");

var main = new Menu("ConnectX Main Menu", EMenuLevel.Root);

// New game uses the SAME instance
main.AddMenuItem("n", "Start New Game", () =>
{
    var controller = new GameController(config);
    controller.GameLoop();
    return "";
});

// Settings edits the SAME instance
var settingsMenu = new SettingsMenu(config);
main.AddMenuItem("s", "Settings", settingsMenu.Run);

// Save current config
main.AddMenuItem("save", "Save Current Configuration", () =>
{
    var savedId = repo.Save(config);
    Console.WriteLine($"Configuration saved as '{savedId}'.");
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
    return "";
});

// Load config -> mutate existing instance (DON'T reassign)
main.AddMenuItem("load", "Load Configuration", () =>
{
    Console.Clear();
    Console.WriteLine("=== Available Configurations ===");
    var ids = repo.List();
    if (ids.Count == 0)
    {
        Console.WriteLine("No configurations found.");
        Console.ReadKey();
        return "";
    }

    for (int i = 0; i < ids.Count; i++) Console.WriteLine($"{i + 1}) {ids[i]}");
    Console.Write("\nEnter number to load: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out var idx) && idx >= 1 && idx <= ids.Count)
    {
        var loaded = repo.Load(ids[idx - 1]);
        config.ApplyFrom(loaded); // <<< key line
        Console.WriteLine($"Loaded '{config.Name}'.");
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }

    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
    return "";
});

// Optional: delete
main.AddMenuItem("del", "Delete Configuration", () =>
{
    Console.Clear();
    Console.WriteLine("=== Delete Configuration ===");
    var ids = repo.List();
    if (ids.Count == 0)
    {
        Console.WriteLine("No configurations found.");
        Console.ReadKey();
        return "";
    }

    for (int i = 0; i < ids.Count; i++) Console.WriteLine($"{i + 1}) {ids[i]}");
    Console.Write("\nEnter number to delete: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out var idx) && idx >= 1 && idx <= ids.Count)
    {
        var id = ids[idx - 1];
        repo.Delete(id);
        Console.WriteLine($"Deleted '{id}'.");
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }

    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
    return "";
});

main.Run();

Console.WriteLine("We are DONE.......");
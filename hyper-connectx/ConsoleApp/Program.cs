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

// TODO: Vii UI asjad UI klassi kõigil alumistel ja varki
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

main.AddMenuItem("loadgame", "Load Saved Game", () =>
{
    var gameRepo = new GameRepositoryJson();
    var saves = gameRepo.List();

    if (saves.Count == 0)
    {
        Console.WriteLine("No saved games found.");
        Console.ReadKey();
        return "";
    }

    Console.Clear();
    Console.WriteLine("=== Saved Games ===");
    for (int i = 0; i < saves.Count; i++)
        Console.WriteLine($"{i + 1}) {saves[i]}");

    Console.Write("\nSelect number to load: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out int index) && index >= 1 && index <= saves.Count)
    {
        var loaded = gameRepo.Load(saves[index - 1]);
        var brain = new GameBrain(loaded.Configuration, loaded.Configuration.P1Name, loaded.Configuration.P2Name);
        brain.LoadGameState(loaded);
        var controller = new GameController(loaded.Configuration)
        {
            GameBrain = brain // you might need to make GameBrain public/internal for this line
        };
        controller.GameLoop();
    }
    else
    {
        Console.WriteLine("Invalid choice.");
        Console.ReadKey();
    }

    return "";
});

main.AddMenuItem("delgame", "Delete Saved Game", () =>
{
    var gameRepo = new GameRepositoryJson();
    Console.Clear();
    Console.WriteLine("=== Delete Saved Game ===");
    var saves = gameRepo.List();
    
    if (saves.Count == 0)
    {
        Console.WriteLine("No saved games found.");
        Console.ReadKey();
        return "";
    }

    for (int i = 0; i < saves.Count; i++)
    {
        Console.WriteLine($"{i + 1}) {saves[i]}");
    }
    
    Console.Write("\nEnter number to delete: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out var idx) && idx >= 1 && idx <= saves.Count)
    {
        var id = saves[idx - 1];
        gameRepo.Delete(id);
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
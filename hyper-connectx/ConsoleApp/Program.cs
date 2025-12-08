using BLL;
using ConsoleApp;
using DAL;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

IRepository<GameConfiguration> configRepo;

// json config repository
configRepo = new ConfigRepositoryJson();
// ef config repository
// using var dbContext = GetDbContext();
// configRepo = new ConfigRepositoryEF(dbContext);

var gameConfig = new GameConfiguration();


Console.WriteLine("Hello, ConnectX!");

var main = new Menu("ConnectX Main Menu", EMenuLevel.Root);

// New game uses the SAME instance
main.AddMenuItem("n", "Start New Game", () =>
{
    var controller = new GameController(gameConfig);
    controller.GameLoop();
    return "";
});

// Settings edits the SAME instance
var settingsMenu = new SettingsMenu(gameConfig);
main.AddMenuItem("s", "Settings", settingsMenu.Run);

// Save current config
main.AddMenuItem("save", "Save Current Configuration", () =>
{
    var savedId = configRepo.Save(gameConfig);
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
    var ids = configRepo.List();
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
        var loaded = configRepo.Load(ids[idx - 1].description);
        gameConfig.ApplyFrom(loaded); // <<< key line
        Console.WriteLine($"Loaded '{gameConfig.Name}'.");
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
    var ids = configRepo.List();
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
        configRepo.Delete(id.id);
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
    IRepository<GameState> gameRepo = new GameRepositoryJson();
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
        var loaded = gameRepo.Load(saves[index - 1].id);
        var brain = new GameBrain(loaded.Configuration);
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
    IRepository<GameState> gameRepo = new GameRepositoryJson();
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
        gameRepo.Delete(id.id);
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


AppDbContext GetDbContext()
{
    // ========================= DB STUFF ========================
    var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    homeDirectory = homeDirectory + Path.DirectorySeparatorChar;

// We are using SQLite
    var connectionString = $"Data Source={homeDirectory}app.db";

    var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(connectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        //.LogTo(Console.WriteLine)
        .Options;

    var resultdbContext = new AppDbContext(contextOptions);
    
    // apply any pending migrations (recreates db as needed)
    resultdbContext.Database.Migrate();
    
    return resultdbContext;
}
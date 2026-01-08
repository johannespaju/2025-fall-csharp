using BLL;
using ConsoleApp;
using DAL;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

IRepository<GameConfiguration> configRepo;
IRepository<GameState> gameRepo;

// Initialize repositories based on central configuration
switch (DatabaseConfig.CurrentProvider)
{
    case EDatabaseProvider.EntityFramework:
    {
        var dbContext = GetDbContext();
        configRepo = new ConfigRepositoryEF(dbContext);
        gameRepo = new GameRepositoryEF(dbContext);
        break;
    }
    case EDatabaseProvider.Json:
    default:
        configRepo = new ConfigRepositoryJson();
        gameRepo = new GameRepositoryJson();
        break;
}

var gameConfig = new GameConfiguration();


Console.WriteLine("Hello, ConnectX!");

var main = new Menu("ConnectX Main Menu", EMenuLevel.Root);

// New game uses the SAME instance
main.AddMenuItem("n", "Start New Game", () =>
{
    Console.Clear();
    Console.WriteLine("Enter Player 1 name: ");
    var p1Name = Console.ReadLine() ?? "Player 1";
    Console.Clear();
    Console.WriteLine("Enter Player 2 name: ");
    var p2Name = Console.ReadLine() ?? "Player 2";
    Console.Clear();
    Console.WriteLine("Press 1 for PvP, 2 for PvC, 3 for CvC");
    var mode = Console.ReadKey(true);
    var gameMode = mode.Key switch
    {
        ConsoleKey.D1 => EGameMode.PvP,
        ConsoleKey.D2 => EGameMode.PvC,
        ConsoleKey.D3 => EGameMode.CvC,
        _ => EGameMode.PvP
    };
    var controller = new GameController(gameConfig, gameRepo, gameMode, p1Name, p2Name);
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

    // Show only description, not the id
    for (int i = 0; i < ids.Count; i++) 
        Console.WriteLine($"{i + 1}) {ids[i].description}");
    
    Console.Write("\nEnter number to load: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out var idx) && idx >= 1 && idx <= ids.Count)
    {
        var loaded = configRepo.Load(ids[idx - 1].id);
        gameConfig.ApplyFrom(loaded);
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

    // Show only description, not the id
    for (int i = 0; i < ids.Count; i++) 
        Console.WriteLine($"{i + 1}) {ids[i].description}");
    
    Console.Write("\nEnter number to delete: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out var idx) && idx >= 1 && idx <= ids.Count)
    {
        var id = ids[idx - 1];
        configRepo.Delete(id.id);
        Console.WriteLine($"Deleted configuration.");
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
    var saves = gameRepo.List();

    if (saves.Count == 0)
    {
        Console.WriteLine("No saved games found.");
        Console.ReadKey();
        return "";
    }

    Console.Clear();
    Console.WriteLine("=== Saved Games ===");
    
    // Show only description, not the id
    for (int i = 0; i < saves.Count; i++)
        Console.WriteLine($"{i + 1}) {saves[i].description}");

    Console.Write("\nSelect number to load: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out int index) && index >= 1 && index <= saves.Count)
    {
        var loaded = gameRepo.Load(saves[index - 1].id);
        var brain = new GameBrain(loaded);
        brain.LoadGameState(loaded);
        var controller = new GameController(loaded.Configuration ?? new GameConfiguration(), gameRepo, loaded.GameMode, loaded.P1Name, loaded.P2Name)
        {
            GameBrain = brain
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
    Console.Clear();
    Console.WriteLine("=== Delete Saved Game ===");
    var saves = gameRepo.List();
    
    if (saves.Count == 0)
    {
        Console.WriteLine("No saved games found.");
        Console.ReadKey();
        return "";
    }

    // Show only description, not the id
    for (int i = 0; i < saves.Count; i++)
    {
        Console.WriteLine($"{i + 1}) {saves[i].description}");
    }
    
    Console.Write("\nEnter number to delete: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out var idx) && idx >= 1 && idx <= saves.Count)
    {
        var id = saves[idx - 1];
        gameRepo.Delete(id.id);
        Console.WriteLine($"Deleted saved game.");
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

    var resultDbContext = new AppDbContext(contextOptions);
    
    // apply any pending migrations (recreates db as needed)
    resultDbContext.Database.Migrate();
    
    return resultDbContext;
}
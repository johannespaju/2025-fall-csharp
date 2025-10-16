using BLL;

namespace MenuSystem;

public class SettingsMenu
{
    private GameConfiguration Configuration { get; set; }

    public SettingsMenu(GameConfiguration configuration)
    {
        Configuration = configuration;
    }

    public string Run()
    {
        var menu = new Menu("Game Settings", EMenuLevel.Second);

        // Add all setting options
        menu.AddMenuItem("1", "Change Board Width", ChangeBoardWidth);
        menu.AddMenuItem("2", "Change Board Height", ChangeBoardHeight);
        menu.AddMenuItem("3", "Change Connect How Many", ChangeConnectHow);
        menu.AddMenuItem("4", "Change Player 1 Name", ChangePlayer1Name);
        menu.AddMenuItem("5", "Change Player 2 Name", ChangePlayer2Name);
        menu.AddMenuItem("6", "Toggle Cylindrical Mode", ToggleCylindricalMode);
        menu.AddMenuItem("7", "Change Game Mode", ChangeGameMode);
        menu.AddMenuItem("8", "Change Configuration Name", ChangeConfigurationName);
        menu.AddMenuItem("9", "Reset to Defaults", ResetToDefaults);
        menu.AddMenuItem("v", "View Current Settings", ViewCurrentSettings);

        return menu.Run();
    }

    private string ChangeBoardWidth()
    {
        Console.Clear();
        Console.WriteLine("=== Change Board Width ===");
        Console.WriteLine($"Current width: {Configuration.BoardWidth}");
        Console.WriteLine();
        Console.Write("Enter new board width (3-20): ");
        
        var input = Console.ReadLine();
        if (int.TryParse(input, out var width) && width >= 3 && width <= 20)
        {
            Configuration.BoardWidth = width;
            
            // Validate ConnectHow is still valid
            if (Configuration.ConnectHow > Math.Min(Configuration.BoardWidth, Configuration.BoardHeight))
            {
                Configuration.ConnectHow = Math.Min(Configuration.BoardWidth, Configuration.BoardHeight);
                Console.WriteLine($"Note: Connect requirement adjusted to {Configuration.ConnectHow} to fit new board size.");
            }
            
            Console.WriteLine($"Board width updated to {width}.");
        }
        else
        {
            Console.WriteLine("Invalid input! Width must be between 3 and 20.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ChangeBoardHeight()
    {
        Console.Clear();
        Console.WriteLine("=== Change Board Height ===");
        Console.WriteLine($"Current height: {Configuration.BoardHeight}");
        Console.WriteLine();
        Console.Write("Enter new board height (3-20): ");
        
        var input = Console.ReadLine();
        if (int.TryParse(input, out var height) && height >= 3 && height <= 20)
        {
            Configuration.BoardHeight = height;
            
            // Validate ConnectHow is still valid
            if (Configuration.ConnectHow > Math.Min(Configuration.BoardWidth, Configuration.BoardHeight))
            {
                Configuration.ConnectHow = Math.Min(Configuration.BoardWidth, Configuration.BoardHeight);
                Console.WriteLine($"Note: Connect requirement adjusted to {Configuration.ConnectHow} to fit new board size.");
            }
            
            Console.WriteLine($"Board height updated to {height}.");
        }
        else
        {
            Console.WriteLine("Invalid input! Height must be between 3 and 20.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ChangeConnectHow()
    {
        Console.Clear();
        Console.WriteLine("=== Change Connect How Many ===");
        Console.WriteLine($"Current connect requirement: {Configuration.ConnectHow}");
        Console.WriteLine($"Board size: {Configuration.BoardWidth}x{Configuration.BoardHeight}");
        
        var maxConnect = Math.Min(Configuration.BoardWidth, Configuration.BoardHeight);
        Console.WriteLine();
        Console.Write($"Enter how many to connect (2-{maxConnect}): ");
        
        var input = Console.ReadLine();
        if (int.TryParse(input, out var connectHow) && connectHow >= 2 && connectHow <= maxConnect)
        {
            Configuration.ConnectHow = connectHow;
            Console.WriteLine($"Connect requirement updated to {connectHow}.");
        }
        else
        {
            Console.WriteLine($"Invalid input! Must be between 2 and {maxConnect}.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ChangePlayer1Name()
    {
        Console.Clear();
        Console.WriteLine("=== Change Player 1 Name ===");
        Console.WriteLine($"Current name: {Configuration.P1Name}");
        Console.WriteLine();
        Console.Write("Enter new name for Player 1: ");
        
        var input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
        {
            Configuration.P1Name = input.Trim();
            Console.WriteLine($"Player 1 name updated to '{Configuration.P1Name}'.");
        }
        else
        {
            Console.WriteLine("Invalid input! Name cannot be empty.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ChangePlayer2Name()
    {
        Console.Clear();
        Console.WriteLine("=== Change Player 2 Name ===");
        Console.WriteLine($"Current name: {Configuration.P2Name}");
        Console.WriteLine();
        Console.Write("Enter new name for Player 2: ");
        
        var input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
        {
            Configuration.P2Name = input.Trim();
            Console.WriteLine($"Player 2 name updated to '{Configuration.P2Name}'.");
        }
        else
        {
            Console.WriteLine("Invalid input! Name cannot be empty.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ToggleCylindricalMode()
    {
        Console.Clear();
        Console.WriteLine("=== Toggle Cylindrical Mode ===");
        Console.WriteLine($"Current mode: {(Configuration.IsCylindrical ? "Cylindrical (edges wrap)" : "Standard (edges are walls)")}");
        Console.WriteLine();
        
        Configuration.IsCylindrical = !Configuration.IsCylindrical;
        
        Console.WriteLine($"Mode changed to: {(Configuration.IsCylindrical ? "Cylindrical" : "Standard")}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ChangeGameMode()
    {
        Console.Clear();
        Console.WriteLine("=== Change Game Mode ===");
        Console.WriteLine($"Current mode: {Configuration.Mode}");
        Console.WriteLine();
        Console.WriteLine("Available modes:");
        Console.WriteLine("1) PvP - Player vs Player");
        Console.WriteLine("2) PvC - Player vs Computer");
        Console.WriteLine("3) CvC - Computer vs Computer");
        Console.WriteLine();
        Console.Write("Select mode (1-3): ");
        
        var input = Console.ReadLine();
        switch (input)
        {
            case "1":
                Configuration.Mode = EGameMode.PvP;
                Console.WriteLine("Game mode updated to Player vs Player.");
                break;
            case "2":
                Configuration.Mode = EGameMode.PvC;
                Console.WriteLine("Game mode updated to Player vs Computer.");
                break;
            case "3":
                Configuration.Mode = EGameMode.CvC;
                Console.WriteLine("Game mode updated to Computer vs Computer.");
                break;
            default:
                Console.WriteLine("Invalid input!");
                break;
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ChangeConfigurationName()
    {
        Console.Clear();
        Console.WriteLine("=== Change Configuration Name ===");
        Console.WriteLine($"Current name: {Configuration.Name}");
        Console.WriteLine();
        Console.Write("Enter new configuration name: ");
        
        var input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
        {
            Configuration.Name = input.Trim();
            Console.WriteLine($"Configuration name updated to '{Configuration.Name}'.");
        }
        else
        {
            Console.WriteLine("Invalid input! Name cannot be empty.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ResetToDefaults()
    {
        Console.Clear();
        Console.WriteLine("=== Reset to Defaults ===");
        Console.WriteLine();
        Console.Write("Are you sure you want to reset all settings to default? (y/n): ");
        
        var input = Console.ReadLine();
        if (input?.ToLower() == "y")
        {
            Configuration.ResetToDefault();
            Console.WriteLine("Settings reset to default values.");
        }
        else
        {
            Console.WriteLine("Reset cancelled.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }

    private string ViewCurrentSettings()
    {
        Console.Clear();
        Console.WriteLine("=== Current Configuration ===");
        Console.WriteLine();
        Console.WriteLine($"Configuration Name: {Configuration.Name}");
        Console.WriteLine($"Board Size: {Configuration.BoardWidth}x{Configuration.BoardHeight}");
        Console.WriteLine($"Connect How Many: {Configuration.ConnectHow}");
        Console.WriteLine($"Player 1 Name: {Configuration.P1Name}");
        Console.WriteLine($"Player 2 Name: {Configuration.P2Name}");
        Console.WriteLine($"Board Type: {(Configuration.IsCylindrical ? "Cylindrical" : "Standard")}");
        Console.WriteLine($"Game Mode: {Configuration.Mode}");
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "";
    }
    
}
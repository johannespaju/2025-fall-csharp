namespace MenuSystem;

public class Menu
{

    private string Title { get; set; } = default!;
    private Dictionary<string, MenuItem> MenuItems { get; set; } = new();
    
    private EMenuLevel Level { get; set; }

    public Menu(string title, EMenuLevel level)
    {
        Title = title;
        Level = level;
        switch (level)
        {
            case EMenuLevel.Root:
                MenuItems["x"] = new MenuItem() {Key = "x",  Value = "Exit", MethodToRun = () => "x"};
                break;
            case EMenuLevel.Second:
                MenuItems["x"] = new MenuItem() {Key = "x",  Value = "Exit", MethodToRun = () => "x" };
                MenuItems["m"] = new MenuItem() {Key = "m",  Value = "Back to Main Menu", MethodToRun = () => "m" };
                break;
            case EMenuLevel.Deep:
                MenuItems["b"] = new MenuItem() {Key = "b", Value = "Back to Previous Menu", MethodToRun = () => "b" };
                MenuItems["m"] = new MenuItem() {Key = "m",  Value = "Back to Main Menu", MethodToRun = () => "m" };
                MenuItems["x"] = new MenuItem() {Key = "x",  Value = "Exit", MethodToRun = () => "x" };
                break;
        }
    }

    public void AddMenuItem(string key, string value, Func<string>? methodToRun)
    {
        if (MenuItems.ContainsKey(key))
        {
            throw new ArgumentException($"Menu item with '{key}' already exists");
        }
        MenuItems[key] = new MenuItem() { Key = key, Value = value, MethodToRun = methodToRun };
    }
    
    public string Run()
    {
        Console.Clear();
        var menuRunning = true;
        var userChoice = "";
        do
        {
            DisplayMenu();
            Console.Write("Select an option: ");
            var input = Console.ReadLine();
            if (input == null)
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            userChoice = input.Trim().ToLower();
            
            if (MenuItems.ContainsKey(userChoice))
            {
                var returnValueFromMethodToRun = MenuItems[userChoice].MethodToRun?.Invoke();

                if (returnValueFromMethodToRun == "b" && Level  == EMenuLevel.Deep)
                {
                    menuRunning = false;
                } else if (returnValueFromMethodToRun == "m" && Level != EMenuLevel.Root)
                {
                    menuRunning = false;
                    userChoice = "m";
                } else if (returnValueFromMethodToRun == "x")
                {
                    menuRunning = false;
                    userChoice = "x";
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
                Thread.Sleep(1000);
            }
        } while (menuRunning);
        return userChoice;
    }

    private void DisplayMenu()
    {
        Console.Clear();
        Console.WriteLine(Title);
        Console.WriteLine("--------------------");
        foreach (var item in MenuItems.Values)
        {
            Console.WriteLine(item);
        }
        Console.WriteLine();
    }
    
}
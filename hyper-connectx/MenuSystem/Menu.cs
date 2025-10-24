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
        AddBackOptions(level);
    }

    private void AddBackOptions(EMenuLevel level)
    {
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
        ConsoleKey keyPressed;
        var menuRunning = true;
        var userChoice = "";
        int selectedIndex = 0;

        var menuItemsList = MenuItems.Values.ToList();

        do
        {
            DisplayMenu(menuItemsList, selectedIndex);

            keyPressed = Console.ReadKey(true).Key;

            switch (keyPressed)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = menuItemsList.Count - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex++;
                    if (selectedIndex >= menuItemsList.Count) selectedIndex = 0;
                    break;
                
                case ConsoleKey.RightArrow:
                case ConsoleKey.Enter:
                    var selectedItem = menuItemsList[selectedIndex];
                    var returnValue = selectedItem.MethodToRun?.Invoke();

                    if (returnValue == "b" && Level == EMenuLevel.Deep)
                    {
                        menuRunning = false;
                        userChoice = "";
                    }
                    else if (returnValue == "m" && Level != EMenuLevel.Root)
                    {
                        menuRunning = false;
                        userChoice = "m";
                    }
                    else if (returnValue == "x")
                    {
                        menuRunning = false;
                        userChoice = "x";
                    }
                    break;

                case ConsoleKey.Escape:
                    // Optional: pressing ESC exits
                    menuRunning = false;
                    userChoice = "x";
                    break;
            }

        } while (menuRunning);

        return userChoice;
    }

    private void DisplayMenu(List<MenuItem> items, int selectedIndex)
    {
        Console.Clear();
        Console.WriteLine(Title);
        Console.WriteLine("--------------------");

        for (int i = 0; i < items.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"> {items[i].Value}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  {items[i].Value}");
            }
        }

        Console.WriteLine("\nUse up and down arrow to navigate, Enter to select");
    }
    
}
namespace MenuSystem;

public class Menu
{
    private List<MenuItem> MenuItems { get; set; } = [];

    public void AddMenuItems(List<MenuItem> items)
    {
        foreach (var item in items)
        {
            // control
            MenuItems.Add(item);
        }
    }
    
    public void Run()
    {
        var menuIsDone = false;
        do
        {
            DisplayMenu();
            Console.Write("Please make a selection: ");
            var userInput = Console.ReadLine();
            // validate
            // execute choice

            if (userInput == "X")
            {
                menuIsDone = true;
            }
        } while (!menuIsDone);
    }

    private void DisplayMenu()
    {
        foreach (MenuItem item in MenuItems)
        {
            Console.WriteLine(item.ToString());
        }
    }
    
}
namespace MenuSystem;

public class Menu
{
    public List<MenuItem> MenuItems { get; set; } = [];


    public void Run()
    {
        var menuIsDone = false;
        do
        {
            DisplayMenu();
            var userInput = Console.ReadLine();
            
            menuIsDone = true;
            
            
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
using BLL;
using ConsoleApp;
using MenuSystem;

var config = new GameConfiguration();

Console.WriteLine("Hello, ConnectX!");

var menu0 = new Menu("ConnectX Main Menu", EMenuLevel.Root);
menu0.AddMenuItem("n", "New game", () =>
{
    var controller = new GameController(config);
    controller.GameLoop();
    
    return "nothing";
});

var menu1 = new Menu("ConnectX Level1 Menu", EMenuLevel.Second);

var menu2 = new Menu("ConnectX Level2 Menu", EMenuLevel.Deep);

var menu3 = new Menu("ConnectX Level3 Menu", EMenuLevel.Deep);

var settingsMenu = new SettingsMenu(config);

menu0.AddMenuItem("1", "Level0 - Go to level1", menu1.Run);
menu0.AddMenuItem("s", "Settings", settingsMenu.Run);

menu1.AddMenuItem("2", "Level1 - Go to level2", menu2.Run);
menu2.AddMenuItem("3", "Level2 - Go to level3", menu3.Run);

menu0.Run();

Console.WriteLine("We are DONE.......");
using MenuSystem;

Console.WriteLine("Hello, ConnectX!");

var mainMenu = new Menu()
{
    MenuItems =
    [
        new MenuItem
        {
            Label = "Label1",
            Key = "1"
        },
        new MenuItem
        {
        Label = "Label2",
        Key = "2"
        },
    ]
};

mainMenu.Run();
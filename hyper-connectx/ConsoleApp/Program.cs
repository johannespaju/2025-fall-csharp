using MenuSystem;

Console.WriteLine("Hello, ConnectX!");

var mainMenu = new Menu();

mainMenu.AddMenuItems(
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
        }
    ]
    );

mainMenu.Run();
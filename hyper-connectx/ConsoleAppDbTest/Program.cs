using ConsoleAppDbTest.DAL;
using ConsoleAppDbTest.Domain;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, DB Demo");

var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
homeDirectory = homeDirectory + Path.DirectorySeparatorChar;

var connectionString = $"Data Source={homeDirectory}app.db";

var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;


// gets disposed correctly, when variable goes out of scope
using var context = new AppDbContext(contextOptions);

var person = new Person(){FirstName = "Juss", LastName = "Jussson"};
context.Persons.Add(person);
context.SaveChanges();

Console.WriteLine(person);

foreach (var dbPerson in context.Persons.Include(p => p.Books)){
    Console.WriteLine(dbPerson);
}

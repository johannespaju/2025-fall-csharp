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

// delete the db
context.Database.EnsureDeleted();
// recreate
context.Database.Migrate();

var author = new Author() { Name = "John Doe" };

var person = new Person() {
    FirstName = "Juss", 
    LastName = "Jussson",
    Books = new List<Book>()
    {
        new Book()
        {
            Title = "The Man who spoke Snakish",
            BookAuthors =  new List<BookAuthor>()
            {
                new BookAuthor()
                {
                    Author = author
                }
            }
        },
        new Book()
        {
        Title = "teine raamat sama autoriga",
        BookAuthors =  new List<BookAuthor>()
        {
            new BookAuthor()
            {
                Author = author
            }
        }
    }
    }
};

context.Persons.Add(person);
context.SaveChanges();

Console.WriteLine(person);
Console.WriteLine(person.Books);


foreach (var dbBook in context.Books
             .Include(p => p.Person!)
             .Include(b => b.BookAuthors!)
             .ThenInclude(ba => ba.Author)
        )

{


    //Console.WriteLine(dbBook);
    //Console.WriteLine(dbBook.Person);

    //context.Entry(dbBook).Collection(p => p.BookAuthors!).Load();

    foreach (var bookAuthor in dbBook.BookAuthors!)
    {
        Console.WriteLine(bookAuthor);
        Console.WriteLine(bookAuthor.Author);
    }
}

using ConsoleAppDbTest.Domain;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppDbTest.DAL;

public class AppDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<Person> Persons { get; set; }
        
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }
}
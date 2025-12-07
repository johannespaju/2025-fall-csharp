using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // remove all the cascade deletion stuff
        foreach (var relationship in modelBuilder.Model
                     .GetEntityTypes()
                     .Where(e => !e.IsOwned())
                     .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
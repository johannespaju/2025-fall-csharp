using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
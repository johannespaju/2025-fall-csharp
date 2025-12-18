using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

// Configure SQLite database connection
var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? $"Data Source={homeDirectory}{Path.DirectorySeparatorChar}app.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());

// Register repository services
builder.Services.AddScoped<IRepository<GameConfiguration>, ConfigRepositoryEF>();
builder.Services.AddScoped<IRepository<GameState>, GameRepositoryEF>();

var app = builder.Build();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

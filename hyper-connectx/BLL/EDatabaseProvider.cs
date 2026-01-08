namespace BLL;

/// <summary>
/// Defines the available database storage providers.
/// </summary>
public enum EDatabaseProvider
{
    /// <summary>Entity Framework Core with SQLite database.</summary>
    EntityFramework,
    
    /// <summary>JSON file-based storage.</summary>
    Json
}

/// <summary>
/// Central configuration for database provider selection.
/// Change the CurrentProvider to switch between EF and JSON storage.
/// </summary>
public static class DatabaseConfig
{
    /// <summary>
    /// Gets or sets the current database provider.
    /// Change this value to switch between EntityFramework and Json storage.
    /// </summary>
    public static EDatabaseProvider CurrentProvider { get; set; } = EDatabaseProvider.EntityFramework;
}

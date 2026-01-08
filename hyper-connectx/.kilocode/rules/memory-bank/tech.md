# Technology: HyperConnectX

## Technology Stack

### Core Framework
- **.NET 10.0** - Target framework for all projects
- **C# (latest)** - Programming language with latest features enabled
- **Nullable reference types** - Enabled project-wide via Directory.Build.props

### Persistence
- **Entity Framework Core 10.0** - ORM for database operations
- **SQLite** - Primary database (via `Microsoft.EntityFrameworkCore.Sqlite`)
- **System.Text.Json** - JSON serialization for file-based storage

### Project Configuration
```xml
<!-- Directory.Build.props -->
<LangVersion>latest</LangVersion>
<Nullable>enable</Nullable>
<WarningsAsErrors>CS8600,CS8602,CS8603,CS8613,CS8618,CS8625</WarningsAsErrors>
```

## Dependencies

### NuGet Packages
| Package | Version | Used By |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 10.0.0 | DAL, ConsoleApp |
| Microsoft.EntityFrameworkCore.Design | 10.0.0 | DAL, ConsoleApp |
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.0 | DAL, ConsoleApp |

### Project References
```
ConsoleApp                  WebApp
├── BLL                     ├── BLL
├── ConsoleUI               └── DAL
│   └── BLL                     └── BLL
├── DAL
│   └── BLL
└── MenuSystem
    └── BLL
```

## Development Setup

### Prerequisites
- .NET 10.0 SDK
- Any C# IDE (Visual Studio, Rider, VS Code with C# extension)

### Building the Project
```bash
# From solution root
dotnet build

# Run the console application
dotnet run --project ConsoleApp

# Run the web application
dotnet run --project WebApp
```

### Database
- SQLite database automatically created at `~/app.db`
- Migrations run automatically on startup via `Database.Migrate()`
- No manual database setup required

### Data Directories
- Configurations: `~/ConnectX/configs/`
- Save games: `~/ConnectX/savegames/`
- Directories created automatically by `FilesystemHelpers`

## Technical Constraints

### Board Limitations
- Width: 3-20 columns
- Height: 3-20 rows
- Connect requirement: 2 to min(width, height)

### AI Constraints
- Minimax depth: 6 (hardcoded, configurable via constructor)
- Evaluation uses window-based scoring
- AI always plays as O (second player) in PvC mode

### Repository Selection
Repository selection is now centralized via `DatabaseConfig.CurrentProvider` in [`BLL/EDatabaseProvider.cs`](BLL/EDatabaseProvider.cs):

```csharp
// Change this to switch storage backend:
public static EDatabaseProvider CurrentProvider { get; set; } = EDatabaseProvider.EntityFramework;
```

Both ConsoleApp and WebApp use this setting to initialize the correct repositories:
```csharp
switch (DatabaseConfig.CurrentProvider)
{
    case EDatabaseProvider.EntityFramework:
        configRepo = new ConfigRepositoryEF(dbContext);
        gameRepo = new GameRepositoryEF(dbContext);
        break;
    case EDatabaseProvider.Json:
    default:
        configRepo = new ConfigRepositoryJson();
        gameRepo = new GameRepositoryJson();
        break;
}
```

- Both EF and JSON repositories expose `ListAsync` for asynchronous retrieval of game listings.
- Full async CRUD methods (`SaveAsync`, `LoadAsync`, `DeleteAsync`) added using EF Core async APIs and `Task.Run` for file‑based repositories.

## Code Quality

### Nullable Reference Types
Strict null checking enabled with specific warnings treated as errors:
- CS8600: Converting null literal or possible null value
- CS8602: Dereference of a possibly null reference
- CS8603: Possible null reference return
- CS8613: Nullability of reference types
- CS8618: Non-nullable field must contain non-null value
- CS8625: Cannot convert null to non-nullable reference type

### Board Representation
- 2D array: `ECellState[width, height]`
- X coordinate = column (0 to width-1)
- Y coordinate = row (0 to height-1, 0 = top)
- Serialized as jagged array for JSON compatibility

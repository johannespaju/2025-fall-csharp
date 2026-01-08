# Architecture: HyperConnectX

## System Architecture Overview

The project follows a layered architecture with clear separation of concerns. The architecture supports multiple front-ends (Console and Web) sharing the same business logic and data access layers.

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                         Presentation Layer                                    │
├─────────────────────────────────┬────────────────────────────────────────────┤
│          ConsoleApp             │              WebApp                         │
│    (Entry Point & Controllers)  │    (ASP.NET Core Razor Pages)              │
├─────────────────────────────────┼────────────────────────────────────────────┤
│  MenuSystem    │   ConsoleUI    │    Pages/     │   wwwroot/                 │
│(Menu Nav)      │(Board Render)  │(Razor Pages)  │(CSS/JS Assets)             │
├─────────────────────────────────┴────────────────────────────────────────────┤
│                               BLL                                             │
│                  (Business Logic Layer - Game Engine)                         │
├──────────────────────────────────────────────────────────────────────────────┤
│                               DAL                                             │
│                    (Data Access Layer - Persistence)                          │
├──────────────────────────────────────────────────────────────────────────────┤
│               SQLite              │           JSON Files                      │
│           (via EF Core)           │         (File System)                     │
└──────────────────────────────────────────────────────────────────────────────┘
```

## Project Structure

```
hyper-connectx/
├── BLL/                    # Business Logic Layer (shared)
│   ├── BaseEntity.cs       # Base class with Guid Id
│   ├── ECellState.cs       # Cell state enum (Empty/X/O/XWin/OWin)
│   ├── EDatabaseProvider.cs # Database provider enum and central config
│   ├── EGameMode.cs        # Game mode enum (PvP/PvC/CvC)
│   ├── GameBrain.cs        # Core game logic
│   ├── GameConfiguration.cs # Configuration entity (rules only)
│   ├── GameState.cs        # Game state entity (includes player names & mode)
│   ├── MinimaxAI.cs        # AI implementation
│   └── MoveResult.cs       # Move calculation result
├── DAL/                    # Data Access Layer (shared)
│   ├── AppDbContext.cs     # EF Core DbContext
│   ├── IRepository.cs      # Repository interface
│   ├── ConfigRepositoryJson.cs
│   ├── ConfigRepositoryEF.cs
│   ├── GameRepositoryJson.cs
│   ├── GameRepositoryEF.cs
│   ├── FilesystemHelpers.cs # Path utilities
│   └── Migrations/         # EF Core migrations
├── ConsoleApp/             # Console application
│   ├── Program.cs          # Entry point
│   └── GameController.cs   # Game loop controller
├── ConsoleUI/              # Console UI rendering
│   └── Ui.cs               # Board drawing methods
├── MenuSystem/             # Console menu framework
│   ├── Menu.cs             # Menu logic
│   ├── MenuItem.cs         # Menu item definition
│   ├── SettingsMenu.cs     # Settings UI (board rules only)
│   └── EMenuLevel.cs       # Menu level enum
├── WebApp/                 # ASP.NET Core Razor Pages
│   ├── Program.cs          # ASP.NET Core setup, DI, middleware
│   ├── WebApp.csproj       # Project file with BLL/DAL references
│   ├── appsettings.json    # App configuration
│   ├── Pages/
│   │   ├── _ViewImports.cshtml # Tag helpers, using directives
│   │   ├── _ViewStart.cshtml   # Layout reference
│   │   ├── Index.cshtml(.cs)   # Home page with game list
│   │   ├── NewGame.cshtml(.cs) # Game creation form with player names & mode
│   │   ├── Game.cshtml(.cs)    # Game board and gameplay
│   │   ├── ConfigManager.cshtml(.cs) # Unified configuration CRUD
│   │   └── Shared/
│   │       └── _Layout.cshtml  # Master layout template
│   └── wwwroot/
│       ├── css/
│       │   └── site.css        # All styling including game board
│       └── js/
│           └── site.js
└── ConsoleAppDbTest/       # Database testing project
```

## Key Components

### BLL - Business Logic Layer

#### [`EDatabaseProvider`](BLL/EDatabaseProvider.cs)
Central database provider configuration:
- `EDatabaseProvider` enum defines available storage options (EntityFramework, Json)
- `DatabaseConfig.CurrentProvider` static property controls repository selection
- Used by both ConsoleApp and WebApp for consistent repository initialization

#### [`GameBrain`](BLL/GameBrain.cs)
Core game engine responsible for:
- Managing board state as `ECellState[,]` 2D array
- Calculating valid moves with [`CalculateMove()`](BLL/GameBrain.cs:37)
- Executing moves with [`ExecuteMove()`](BLL/GameBrain.cs:68)
- Win detection via [`GetWinner()`](BLL/GameBrain.cs:120) - also marks all winning cells with XWin/OWin states
- Cylindrical board wrapping via [`WrapX()`](BLL/GameBrain.cs:108)
- Marking winning cells via [`MarkWinner()`](BLL/GameBrain.cs) - marks all cells in a winning line
- Game state serialization/deserialization
- Game mode access via [`GetGameMode()`](BLL/GameBrain.cs:227)

Constructor accepts `GameState` (not `GameConfiguration`) to provide access to both game rules, player names, and game mode.

#### [`MinimaxAI`](BLL/MinimaxAI.cs)
AI opponent using Minimax algorithm:
- Alpha-beta pruning optimization
- Configurable search depth (default: 6)
- Positional evaluation with center column preference
- Window-based scoring for partial lines

#### [`GameConfiguration`](BLL/GameConfiguration.cs)
Configuration entity storing **rules only**:
- Board dimensions (width/height)
- Connect requirement
- Cylindrical mode flag

**Note:** No longer stores GameMode - configurations are now purely game rule templates.

#### [`GameState`](BLL/GameState.cs)
Game save state containing:
- Configuration reference
- Board state (serialized as JSON)
- Next player turn
- Save name with timestamp
- Player names (P1Name, P2Name)
- Game mode (EGameMode)

### DAL - Data Access Layer

#### Repository Pattern
- [`IRepository<TData>`](DAL/IRepository.cs) - Generic repository interface
- Dual implementation: JSON files and Entity Framework Core
- SQLite database stored at `~/app.db`
- JSON files stored at `~/ConnectX/configs/` and `~/ConnectX/savegames/`
- Async support: `ListAsync` method added to both EF and JSON repositories for non‑blocking retrieval. Full async CRUD methods (`SaveAsync`, `LoadAsync`, `DeleteAsync`) now implemented.

### ConsoleApp

#### [`GameController`](ConsoleApp/GameController.cs)
Main game loop handling:
- Player input (arrow keys for column selection)
- AI turn management via [`ShouldAiMove()`](ConsoleApp/GameController.cs:192) and [`MakeAiMove()`](ConsoleApp/GameController.cs:214)
- Piece drop animation via [`AnimatePieceFalling()`](ConsoleApp/GameController.cs:269)
- Win/draw detection
- Game saving functionality

Constructor now accepts `EGameMode`, `p1Name`, and `p2Name` parameters.

### MenuSystem

#### [`Menu`](MenuSystem/Menu.cs)
Arrow key-navigable menu system:
- Three levels: Root, Second, Deep
- Automatic back/exit navigation options
- Visual highlighting of selected item

#### [`SettingsMenu`](MenuSystem/SettingsMenu.cs)
Now only configures board rules (player names and mode are per-game settings).

### WebApp

ASP.NET Core Razor Pages web application providing browser-based gameplay.

#### [`Program.cs`](WebApp/Program.cs)
Application entry point and configuration:
- DI registration for DbContext and repositories based on `DatabaseConfig.CurrentProvider`
- Middleware pipeline configuration
- Razor Pages service registration

#### Pages Structure
- [`Index`](WebApp/Pages/Index.cshtml.cs) - Home page listing saved games with load/delete actions
- [`NewGame`](WebApp/Pages/NewGame.cshtml.cs) - Game creation form with configuration selection, player names, and game mode
- [`Game`](WebApp/Pages/Game.cshtml.cs) - Core gameplay page with board rendering and move handling:
  - `OnPostMove()` - Handles human player moves with immediate winner detection
  - `OnPostAiMove()` - Handles AI moves with immediate winner detection
  - `FindWinner()` - Scans board for XWin/OWin cells to determine winner
  - POST handlers redirect to game page when game is over, preventing additional moves
- [`ConfigManager`](WebApp/Pages/ConfigManager.cshtml.cs) - Unified configuration CRUD

#### Styling
- [`site.css`](WebApp/wwwroot/css/site.css) - Comprehensive styling including:
  - CSS Grid-based responsive game board
  - Form styling for configuration management
  - Navigation and layout styles

## Design Patterns

| Pattern | Implementation |
|---------|----------------|
| Repository | `IRepository<T>` with JSON and EF implementations |
| Factory | `MoveResult.Valid()` and `MoveResult.Invalid()` |
| Strategy | Repository injection (JSON vs EF) via `DatabaseConfig.CurrentProvider` |
| MVC-like | GameBrain (Model), Ui (View), GameController (Controller) |

## Data Flow

```
User Input → GameController → GameBrain → MoveResult
                ↓                ↓
              Ui.DrawBoard    Repository.Save
```

## Critical Implementation Paths

### Making a Move
1. [`GameController`](ConsoleApp/GameController.cs) captures arrow key input
2. [`GameBrain.CalculateMove()`](BLL/GameBrain.cs:37) validates and calculates landing row
3. [`GameController.AnimatePieceFalling()`](ConsoleApp/GameController.cs:269) shows animation
4. [`GameBrain.ExecuteMove()`](BLL/GameBrain.cs:68) places piece and switches turn
5. [`GameBrain.GetWinner()`](BLL/GameBrain.cs:120) checks for win condition

### AI Move
1. [`GameController.ShouldAiMove()`](ConsoleApp/GameController.cs:192) checks if AI should play based on `GameBrain.GetGameMode()`
2. [`MinimaxAI.GetBestMove()`](BLL/MinimaxAI.cs:22) evaluates all possible moves
3. [`MinimaxAI.Minimax()`](BLL/MinimaxAI.cs:57) recursively searches game tree
4. Best column returned and move executed normally

### Repository Selection
1. [`EDatabaseProvider`](BLL/EDatabaseProvider.cs) enum defines options
2. [`DatabaseConfig.CurrentProvider`](BLL/EDatabaseProvider.cs:25) static property controls selection
3. Both [`ConsoleApp/Program.cs`](ConsoleApp/Program.cs:11) and [`WebApp/Program.cs`](WebApp/Program.cs:22) use switch statement to select implementation

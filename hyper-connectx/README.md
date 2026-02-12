# HyperConnectX (ConnectX)

HyperConnectX is a customizable **Connect Four-style** game built with **C# / .NET 10**. It supports both a **console UI** and a **web UI** (ASP.NET Core Razor Pages), sharing the same game engine and persistence layer.

- **Solution file:** [`hyper-connectx.sln`](hyper-connectx.sln)
- **Core engine:** [`GameBrain`](BLL/GameBrain.cs:1)

## Features

- **Custom rules**
  - Board size: **3–20** columns/rows
  - Win condition: **Connect X** (2 to min(width, height))
  - Optional **cylindrical board** (horizontal wrapping)
- **Game modes**
  - Player vs Player (PvP)
  - Player vs Computer (PvC)
  - Computer vs Computer (CvC)
  - Stored per game in [`EGameMode`](BLL/EGameMode.cs:1) / [`GameState.GameMode`](BLL/GameState.cs:1)
- **AI opponent**
  - Minimax with alpha-beta pruning via [`MinimaxAI`](BLL/MinimaxAI.cs:1)
  - Difficulty levels: **Easy / Medium / Hard** via [`EAiDifficulty`](BLL/EAiDifficulty.cs:1)
- **Persistence (switchable)**
  - **SQLite** via **Entity Framework Core**
  - **JSON files** on disk
  - Provider selection centralized in [`DatabaseConfig.CurrentProvider`](BLL/EDatabaseProvider.cs:1)
- **Save/load**
  - Save/load games and configurations through the shared repository layer in [`DAL`](DAL/:1)

## Architecture (high level)

The solution follows a layered architecture with shared business logic and persistence used by both front-ends:

- **Presentation**
  - Console: [`ConsoleApp`](ConsoleApp/:1) + [`ConsoleUI`](ConsoleUI/:1) + [`MenuSystem`](MenuSystem/:1)
  - Web: [`WebApp`](WebApp/:1) (ASP.NET Core Razor Pages)
- **Business Logic Layer (BLL)**
  - Game rules, board state, win detection, AI
  - Key types: [`GameBrain`](BLL/GameBrain.cs:1), [`GameState`](BLL/GameState.cs:1), [`GameConfiguration`](BLL/GameConfiguration.cs:1)
- **Data Access Layer (DAL)**
  - Repository pattern with EF Core + JSON implementations
  - Key types: [`AppDbContext`](DAL/AppDbContext.cs:1), [`IRepository<T>`](DAL/IRepository.cs:1)

## Prerequisites

- **.NET 10 SDK**
- (Optional) A C# IDE such as Visual Studio / Rider / VS Code with C# tooling

## Build

From the repository root:

```bash
dotnet build
```

## Run: ConsoleApp

```bash
dotnet run --project ConsoleApp
```

Basic notes:
- Use the menu system to start a new game, load a saved game, and adjust rule configurations.
- In AI modes, you can select difficulty (Easy/Medium/Hard).

## Run: WebApp

```bash
dotnet run --project WebApp
```

Then open the URL shown in the console output (typically `http://localhost:xxxx`).

Web UI includes:
- Creating a new game (choose configuration, player names, mode, AI difficulty)
- Playing the game in the browser
- Managing configurations and saved games

## Switching storage provider (SQLite EF Core vs JSON)

Storage backend selection is centralized in [`DatabaseConfig.CurrentProvider`](BLL/EDatabaseProvider.cs:1).

1. Open [`EDatabaseProvider.cs`](BLL/EDatabaseProvider.cs:1)
2. Change the provider:

```csharp
// Change this to switch storage backend:
public static EDatabaseProvider CurrentProvider { get; set; } = EDatabaseProvider.EntityFramework;
// or
// public static EDatabaseProvider CurrentProvider { get; set; } = EDatabaseProvider.Json;
```

Notes:
- Both the console and web apps use this same setting.
- When using EF Core, migrations are applied on startup (SQLite database is created/updated automatically).

## Where data is stored

### JSON provider

Stored under your user home directory:

- Configurations: `~/ConnectX/configs/`
- Save games: `~/ConnectX/savegames/`

Directory creation is handled by [`FilesystemHelpers`](DAL/FilesystemHelpers.cs:1).

### EF Core (SQLite) provider

- SQLite database file: `~/app.db`

EF Core setup lives in [`AppDbContext`](DAL/AppDbContext.cs:1) and migrations are in [`DAL/Migrations`](DAL/Migrations/:1).

## Project layout

- [`BLL`](BLL/:1) — game engine, rules, AI
- [`DAL`](DAL/:1) — repositories (EF Core + JSON), migrations
- [`ConsoleApp`](ConsoleApp/:1) — console entry point + game loop controller
- [`ConsoleUI`](ConsoleUI/:1) — console rendering utilities
- [`MenuSystem`](MenuSystem/:1) — reusable console menu framework
- [`WebApp`](WebApp/:1) — ASP.NET Core Razor Pages front-end

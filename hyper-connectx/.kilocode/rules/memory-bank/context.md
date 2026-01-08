# Context: HyperConnectX

## Current State

The project is a fully functional Connect X game with:
- Complete core game logic (BLL)
- Working data access layer with dual repository support (JSON and EF Core)
- Console application with animated piece dropping and menu system
- **Web application** using ASP.NET Core Razor Pages (completed 2025-12-18, updated 2026-01-08)
- AI opponent using Minimax with alpha-beta pruning

Both ConsoleApp and WebApp share the same BLL and DAL layers.

## Recent Changes

- Memory bank initialized on 2025-12-17
- 2025-12-18: WebApp implementation completed
- 2026-01-07: Major refactoring - Player names moved from Configuration to GameState
- **2026-01-08: Major refactoring - GameMode moved from Configuration to GameState**
  - New database migration `MoveGameModeToGameState` removes `Mode` from `GameConfigurations` and adds `GameMode` to `GameStates` table
  - [`GameConfiguration`](BLL/GameConfiguration.cs) is now purely rule-based (board size, connect requirement, cylindrical mode only)
  - [`GameState`](BLL/GameState.cs) now has `EGameMode GameMode` property (defaulting to `EGameMode.PvP`)
  - [`GameBrain`](BLL/GameBrain.cs) reads game mode from `GameState`, not configuration
  - New file [`BLL/EDatabaseProvider.cs`](BLL/EDatabaseProvider.cs) - Central database provider configuration enum and static config
  - Both [`WebApp/Program.cs`](WebApp/Program.cs) and [`ConsoleApp/Program.cs`](ConsoleApp/Program.cs) now use `DatabaseConfig.CurrentProvider` to select repository implementation
  - Repository switching is now centralized - change `DatabaseConfig.CurrentProvider` in `BLL/EDatabaseProvider.cs` to switch storage
  - [`NewGame.cshtml.cs`](WebApp/Pages/NewGame.cshtml.cs) collects player names AND game mode at game creation time
  - [`ConsoleApp/Program.cs`](ConsoleApp/Program.cs) prompts for player names and game mode when starting a new game
  - [`GameController`](ConsoleApp/GameController.cs) constructor now takes `EGameMode`, `p1Name`, and `p2Name` parameters

## Active Work Focus

**Configurations are now pure rule templates** - All per-game settings (player names, game mode) are in GameState:
- `GameConfiguration` defines board rules: width, height, connect requirement, cylindrical mode
- `GameState` holds game-specific data: player names, game mode, board state, turn
- Repository selection is centralized via `DatabaseConfig.CurrentProvider` enum
- Both ConsoleApp and WebApp now prompt for player names and game mode at game start

## Open Questions / Technical Debt

1. **TODO in ConsoleApp/Program.cs**: "Vii UI asjad UI klassi kõigil alumistel ja varki" (Estonian: "Move UI stuff to UI class for all the lower ones and such") - suggests UI code should be refactored out of Program.cs into the ConsoleUI project.

2. **Debug statements**: There are debug `Console.WriteLine` statements in:
   - [`GameRepositoryEF.Load()`](DAL/GameRepositoryEF.cs:68)
   - [`ConfigRepositoryEF.Load()`](DAL/ConfigRepositoryEF.cs:62)

3. **Hardcoded AI depth**: The Minimax depth is set at construction time but defaults to 6 - could be made configurable via settings.

4. **WebApp AI delay**: No artificial delay for AI moves - AI responds instantly which may feel abrupt to users.

5. **WebApp PvP sharing**: Two players on the same device work fine, but remote PvP requires manual URL sharing and page refresh to see opponent moves.

## Next Steps

1. **Optional enhancements** (not required):
   - Add SignalR for real-time game updates in WebApp
   - Add game lobby/matchmaking system
   - Make AI depth configurable in WebApp
   - Add game history/replay feature

## Dependencies Status

All dependencies are at version 10.0.0 (Entity Framework Core packages), aligning with .NET 10.0 target framework.

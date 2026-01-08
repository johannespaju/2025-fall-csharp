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
- **2026-01-08: WebApp Win Detection Bug Fixed**
  - Added [`MarkWinner()`](BLL/GameBrain.cs) method to `GameBrain` to mark all winning cells in a winning line
  - Modified [`GetWinner()`](BLL/GameBrain.cs) to track and mark all winning cells on the board
  - Removed `LastMove` property from [`Game.cshtml.cs`](WebApp/Pages/Game.cshtml.cs) as it's no longer needed
  - Added immediate winner detection in `OnPostMove()` and `OnPostAiMove()` handlers
  - Updated [`FindWinner()`](WebApp/Pages/Game.cshtml.cs) to scan for XWin/OWin cells on the board
  - POST handlers now redirect to game page when game is over, preventing additional moves after a win
  - Game now properly highlights winning pieces and ends the game correctly
- **2026-01-08: IsHidden Feature Added**
  - Added `IsHidden` property to [`GameConfiguration`](BLL/GameConfiguration.cs) to allow hiding configurations from UI
  - New database migration `AddIsHiddenToGameConfiguration` adds `IsHidden` column to `GameConfigurations` table
  - Configurations can be hidden while preserving existing games that use them
- **2026-01-08: Database Schema Mismatch Bug Fixed**
  - Added `LastMoveColumn` and `LastMoveRow` properties to [`BLL/GameState.cs`](BLL/GameState.cs:27)
  - These properties were added in migration `20260108194806_AddLastMoveToGameState` but were missing from the entity class
  - A subsequent migration `20260108195505_fix` dropped these columns from the database
  - To persist these values, a new EF Core migration will need to be created
- **2026-01-08: GameStatus Feature Added**
  - New [`EGameStatus`](BLL/EGameStatus.cs) enum with values: `InProgress`, `XWon`, `OWon`, `Draw`
  - Added `Status` property to [`GameState`](BLL/GameState.cs:30) entity
  - Database migration `AddGameStatus` adds Status column to GameStates table
  - Enables explicit game status tracking and querying by game state
- **2026-01-08: AI Difficulty Levels Added**
  - New [`EAiDifficulty`](BLL/EAiDifficulty.cs) enum with values: `Easy`, `Medium`, `Hard`
  - Added `Difficulty` property to [`GameState`](BLL/GameState.cs) entity (default: `Hard`)
  - Updated [`MinimaxAI`](BLL/MinimaxAI.cs) to accept difficulty parameter:
    - **Easy**: Random valid move (no minimax evaluation)
    - **Medium**: Depth 4 with standard evaluation
    - **Hard**: Depth 6 with standard evaluation (previous behavior)
  - Updated [`GameBrain`](BLL/GameBrain.cs) with `GetDifficulty()` method
  - Updated [`ConsoleApp/GameController.cs`](ConsoleApp/GameController.cs) to prompt for difficulty in AI modes
  - Updated [`ConsoleApp/Program.cs`](ConsoleApp/Program.cs) with difficulty selection menu
  - Updated [`WebApp/NewGame.cshtml`](WebApp/Pages/NewGame.cshtml) with difficulty dropdown
  - Updated [`WebApp/NewGame.cshtml.cs`](WebApp/Pages/NewGame.cshtml.cs) to save difficulty in GameState
  - Updated [`WebApp/Game.cshtml.cs`](WebApp/Pages/Game.cshtml.cs) to use difficulty when creating AI
  - Database migration `AddAiDifficulty` adds Difficulty column to GameStates table

## Active Work Focus

**Configurations are now pure rule templates** - All per-game settings (player names, game mode, game status) are in GameState:
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

4. **~~AI Difficulty Levels~~**: **RESOLVED** - Added EAiDifficulty enum with Easy, Medium, Hard options.

5. **WebApp AI delay**: No artificial delay for AI moves - AI responds instantly which may feel abrupt to users.

6. **WebApp PvP sharing**: Two players on the same device work fine, but remote PvP requires manual URL sharing and page refresh to see opponent moves.

6. ~~LastMove columns not persisted~~: **RESOLVED** - Migration `AddLastMoveBack` re-added the `LastMoveColumn` and `LastMoveRow` columns to the database. The entity properties in `GameState.cs` now match the database schema.

## Next Steps

1. **Optional enhancements** (not required):
   - Add SignalR for real-time game updates in WebApp
   - Add game lobby/matchmaking system
   - Add game history/replay feature

## Dependencies Status

All dependencies are at version 10.0.0 (Entity Framework Core packages), aligning with .NET 10.0 target framework.

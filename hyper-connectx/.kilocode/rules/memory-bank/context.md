# Context: HyperConnectX

## Current State

The project is a fully functional Connect X game with:
- Complete core game logic (BLL)
- Working data access layer with dual repository support (JSON and EF Core)
- Console application with animated piece dropping and menu system
- **Web application** using ASP.NET Core Razor Pages (completed 2025-12-18)
- AI opponent using Minimax with alpha-beta pruning

Both ConsoleApp and WebApp share the same BLL and DAL layers.

## Recent Changes

- Memory bank initialized on 2025-12-17
- 2025-12-18: WebApp implementation completed with:
  - Home page with saved games list (Index)
  - New game creation form with configuration options (NewGame)
  - Game board rendering with CSS Grid and move handling (Game)
  - Full Configuration CRUD pages (Create/Read/Update/Delete)
  - Shared layout with navigation
  - CSS styling for responsive game board

## Active Work Focus

**WebApp Complete** - The ASP.NET Core Razor Pages web application is fully implemented:
- Reuses existing BLL (GameBrain, MinimaxAI, GameConfiguration, GameState)
- Reuses existing DAL (IRepository with EF Core implementation via DI)
- Supports PvP (Player vs Player) and PvC (Player vs Computer) modes
- Uses unique game URLs for access (/Game?id={guid})
- Page refresh for game state updates (no real-time WebSocket/SignalR)
- Minimal JavaScript (no JS required for core functionality)

## Open Questions / Technical Debt

1. **TODO in ConsoleApp/Program.cs**: "Vii UI asjad UI klassi kõigil alumistel ja varki" (Estonian: "Move UI stuff to UI class for all the lower ones and such") - suggests UI code should be refactored out of Program.cs into the ConsoleUI project.

2. **Debug statements**: There are debug `Console.WriteLine` statements in:
   - [`GameRepositoryEF.Load()`](DAL/GameRepositoryEF.cs:68)
   - [`ConfigRepositoryEF.Load()`](DAL/ConfigRepositoryEF.cs:62)

3. **Hardcoded AI depth**: The Minimax depth is set at construction time but defaults to 6 - could be made configurable via settings.

4. **Repository switching in ConsoleApp**: JSON vs EF Core is currently selected by commenting/uncommenting code in Program.cs - could be made configurable.

5. **WebApp AI delay**: No artificial delay for AI moves - AI responds instantly which may feel abrupt to users.

6. **WebApp PvP sharing**: Two players on the same device work fine, but remote PvP requires manual URL sharing and page refresh to see opponent moves.

## Next Steps

1. **Optional enhancements** (not required):
   - Add SignalR for real-time game updates in WebApp
   - Add game lobby/matchmaking system
   - Make AI depth configurable in WebApp
   - Add game history/replay feature

## Dependencies Status

All dependencies are at version 10.0.0 (Entity Framework Core packages), aligning with .NET 10.0 target framework.

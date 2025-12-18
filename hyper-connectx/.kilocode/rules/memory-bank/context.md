# Context: HyperConnectX

## Current State

The project is a functional Connect X console game with:
- Complete core game logic (BLL)
- Working data access layer with dual repository support (JSON and EF Core)
- Console UI with animated piece dropping
- Menu system with settings management
- AI opponent using Minimax with alpha-beta pruning

## Recent Changes

- Memory bank initialized on 2025-12-17
- 2025-12-18: Planning ASP.NET Core Razor Pages web application

## Active Work Focus

**Implementing WebApp** - ASP.NET Core Razor Pages web application that:
- Reuses existing BLL (GameBrain, MinimaxAI, GameConfiguration, GameState)
- Reuses existing DAL (IRepository with both JSON and EF Core implementations)
- Supports PvP (Player vs Player) and PvC (Player vs Computer) modes
- Uses unique game URLs for access (no lobby system)
- Page refresh for game state updates (no real-time WebSocket/SignalR)
- Minimal JavaScript (only for essential interactions)

## Open Questions / Technical Debt

1. **TODO in Program.cs (line 47)**: "Vii UI asjad UI klassi kõigil alumistel ja varki" (Estonian: "Move UI stuff to UI class for all the lower ones and such") - suggests UI code should be refactored out of Program.cs into the ConsoleUI project.

2. **Debug statements**: There are debug `Console.WriteLine` statements in:
   - [`GameRepositoryEF.Load()`](DAL/GameRepositoryEF.cs:68)
   - [`ConfigRepositoryEF.Load()`](DAL/ConfigRepositoryEF.cs:62)

3. **Hardcoded AI depth**: The Minimax depth is set at construction time but defaults to 6 - could be made configurable via settings.

4. **Repository switching**: JSON vs EF Core is currently selected by commenting/uncommenting code in Program.cs - could be made configurable.

## Next Steps

1. Create WebApp project with ASP.NET Core Razor Pages
2. Implement game board rendering in HTML/CSS
3. Add configuration management pages
4. Add game state persistence integration

## Dependencies Status

All dependencies are at version 10.0.0 (Entity Framework Core packages), aligning with .NET 10.0 target framework.

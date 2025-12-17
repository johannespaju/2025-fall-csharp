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
- No recent code changes documented

## Active Work Focus

None currently - project appears to be in a stable/complete state for the console application.

## Open Questions / Technical Debt

1. **TODO in Program.cs (line 47)**: "Vii UI asjad UI klassi kõigil alumistel ja varki" (Estonian: "Move UI stuff to UI class for all the lower ones and such") - suggests UI code should be refactored out of Program.cs into the ConsoleUI project.

2. **Debug statements**: There are debug `Console.WriteLine` statements in:
   - [`GameRepositoryEF.Load()`](DAL/GameRepositoryEF.cs:68)
   - [`ConfigRepositoryEF.Load()`](DAL/ConfigRepositoryEF.cs:62)

3. **Hardcoded AI depth**: The Minimax depth is set at construction time but defaults to 6 - could be made configurable via settings.

4. **Repository switching**: JSON vs EF Core is currently selected by commenting/uncommenting code in Program.cs - could be made configurable.

## Next Steps

Potential enhancements that could be added:
- Web application version (WebApp folder was mentioned but not present)
- Configurable AI difficulty
- Settings persistence between sessions
- Network multiplayer support

## Dependencies Status

All dependencies are at version 10.0.0 (Entity Framework Core packages), aligning with .NET 10.0 target framework.

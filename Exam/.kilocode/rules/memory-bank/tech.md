# Tech

## Stack
- **Language/runtime**: C# with .NET target frameworks set to `net10.0` (see [`Webapp/Webapp.csproj`](Webapp/Webapp.csproj:1), [`DAL/DAL.csproj`](DAL/DAL.csproj:1), [`BLL/BLL.csproj`](BLL/BLL.csproj:1)).
- **Web**: ASP.NET Core Razor Pages (see [`Webapp.Program`](Webapp/Program.cs:1)).
- **ORM**: Entity Framework Core 10.0.2 (see [`DAL/DAL.csproj`](DAL/DAL.csproj:1)).
- **Database**: SQLite intended (design-time factory uses `UseSqlite`), with SQL Server provider referenced as well.
- **Frontend**: Bootstrap (scaffolded layout) + jQuery (see [`Webapp/Pages/Shared/_Layout.cshtml`](Webapp/Pages/Shared/_Layout.cshtml:1)).

## Development setup and workflow
- Build entire solution:
  - `dotnet build` (from repo root)
- Run web app:
  - `dotnet run --project Webapp`

## EF Core tooling notes
- Migrations use the design-time factory [`DAL.AppDbContextFactory.CreateDbContext()`](DAL/AppDbContextFactory.cs:8).
- SQLite DB path used by tooling:
  - `${USERPROFILE}/exam/app.db` (Windows example: `C:\Users\\<user>\\exam\\app.db`).

## Repo-wide constraints
- Nullability is enabled and several warnings are treated as errors (see [`Directory.Build.props`](Directory.Build.props:1)).


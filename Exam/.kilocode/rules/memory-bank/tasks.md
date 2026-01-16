# Tasks

## Run the web app locally
**Goal:** Start the Razor Pages application.

1. From repo root, run: `dotnet run --project Webapp`.
2. Open the printed URL in the browser.

## Add an EF Core migration (SQLite)
**Goal:** Create migrations after implementing `DbSet<>`s and entity properties.

1. Ensure [`DAL.AppDbContext`](DAL/AppDbContext.cs:5) contains the required `DbSet<>`s and mappings.
2. Use the factory-backed context from [`DAL.AppDbContextFactory.CreateDbContext()`](DAL/AppDbContextFactory.cs:8).
3. Run EF tooling (example):
   - `dotnet ef migrations add InitialCreate --project DAL`
   - `dotnet ef database update --project DAL`

**Notes:** Commands may need `--startup-project Webapp` once `Webapp` references `DAL` and configures DI.


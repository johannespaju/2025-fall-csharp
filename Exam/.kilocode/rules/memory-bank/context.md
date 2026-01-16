# Context

## Current work focus
- Project is currently a scaffolded solution with placeholder domain entities and minimal web UI.
- Next work is to implement the domain model fields, EF Core persistence, and core business logic (availability, pricing, deposits, maintenance flagging).

## Recent changes
- Memory Bank initialized (core documentation created).

## Next steps
1. Define BLL entities with required properties and relationships (Bike, Rental, Customer, Tour, TourBooking, MaintenanceRecord, DamageRecord).
2. Implement `DbSet<>`s + relationships in [`DAL.AppDbContext`](DAL/AppDbContext.cs:5) and wire it into [`Webapp.Program`](Webapp/Program.cs:1).
3. Add migrations and create the SQLite database (see [`DAL.AppDbContextFactory`](DAL/AppDbContextFactory.cs:6)).
4. Build Razor Pages CRUD + availability checking workflows.


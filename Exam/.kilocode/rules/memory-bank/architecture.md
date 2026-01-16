# Architecture

## Solution structure
The solution is split into three projects (see [`Exam.sln`](Exam.sln)):

- **Webapp**: ASP.NET Core Razor Pages UI layer.
  - Entry point: [`Webapp.Program`](Webapp/Program.cs:1)
  - Pages: [`Webapp/Pages`](Webapp/Pages/Index.cshtml:1)

- **BLL**: Business/domain layer (currently placeholder entities).
  - Base entity: [`BLL.BaseEntity`](BLL/BaseEntity.cs:3)
  - Domain entities: [`BLL.Bike`](BLL/Bike.cs:3), [`BLL.Rental`](BLL/Rental.cs:3), [`BLL.Customer`](BLL/Customer.cs:3), [`BLL.Tour`](BLL/Tour.cs:3), [`BLL.TourBooking`](BLL/TourBooking.cs:3), [`BLL.MaintenanceRecord`](BLL/MaintenanceRecord.cs:3), [`BLL.DamageRecord`](BLL/DamageRecord.cs:3)

- **DAL**: Data access layer (EF Core) (currently minimal).
  - DbContext: [`DAL.AppDbContext`](DAL/AppDbContext.cs:5)
  - EF design-time factory (migrations): [`DAL.AppDbContextFactory.CreateDbContext()`](DAL/AppDbContextFactory.cs:8)

## Intended layering and responsibilities
- **Webapp**
  - Handles HTTP + Razor Pages + UI validation.
  - Calls into BLL services (to be created) to perform operations.

- **BLL**
  - Domain model + business rules:
    - availability/overlap detection
    - rental extension rules
    - tour capacity rules
    - pricing calculation (4h blocks / full-day)
    - deposit calculation (bike type + customer damage history)
    - maintenance flagging (threshold - buffer)

- **DAL**
  - EF Core persistence, mapping, migrations.
  - SQLite is the default target database for the exam setup.

## Key technical decisions (current state)
- EF Core is present, but `DbSet<>`s and model mapping are not implemented yet in [`DAL.AppDbContext`](DAL/AppDbContext.cs:5).
- Design-time factory uses a per-user SQLite file path for migrations tooling.

## Critical implementation paths (to implement)
1. **Availability checking**
   - Given a requested time window, detect overlap with existing rentals for a bike.
2. **Return workflow**
   - Update odometer, create maintenance/damage records, and update bike availability.
3. **Tour booking workflow**
   - Enforce capacity; create rentals for participants; handle upgrades.


# Architecture

## Solution structure
The solution is split into three projects (see [`Exam.sln`](Exam.sln)):

- **Webapp**: ASP.NET Core Razor Pages UI layer.
  - Entry point: [`Webapp.Program`](Webapp/Program.cs:1)
  - Pages: 
    - Bikes: [`Create`](Webapp/Pages/Bikes/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Bikes/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Bikes/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Bikes/Delete.cshtml.cs:1)
    - Customers: [`Create`](Webapp/Pages/Customers/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Customers/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Customers/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Customers/Delete.cshtml.cs:1)
    - Rentals: [`Create`](Webapp/Pages/Rentals/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Rentals/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Rentals/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Rentals/Delete.cshtml.cs:1), [`Extend`](Webapp/Pages/Rentals/Extend.cshtml.cs:1), [`Return`](Webapp/Pages/Rentals/Return.cshtml.cs:1)
    - Tours: [`Create`](Webapp/Pages/Tours/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Tours/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Tours/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Tours/Delete.cshtml.cs:1), [`Book`](Webapp/Pages/Tours/Book.cshtml.cs:1)

- **BLL**: Business/domain layer with complete entity and service implementation.
  - Base entity: [`BLL.BaseEntity`](BLL/BaseEntity.cs:3)
  - Domain entities: 
    - [`BLL.Bike`](BLL/Bike.cs:3) - bike inventory with type, pricing, and maintenance tracking
    - [`BLL.Rental`](BLL/Rental.cs:3) - rental records with time windows and pricing
    - [`BLL.Customer`](BLL/Customer.cs:3) - customer information and history
    - [`BLL.Tour`](BLL/Tour.cs:3) - guided tour offerings with capacity
    - [`BLL.TourBooking`](BLL/TourBooking.cs:3) - tour reservations with participants
    - [`BLL.MaintenanceRecord`](BLL/MaintenanceRecord.cs:3) - maintenance history
    - [`BLL.DamageRecord`](BLL/DamageRecord.cs:3) - damage history

  - Services (interfaces + implementations):
    - [`BLL.Interfaces.IAvailabilityService`](BLL/Interfaces/IAvailabilityService.cs:1) / [`BLL.Services.AvailabilityService`](BLL/Services/AvailabilityService.cs:1) - checks bike availability and rental conflicts
    - [`BLL.Interfaces.IPricingService`](BLL/Interfaces/IPricingService.cs:1) / [`BLL.Services.PricingService`](BLL/Services/PricingService.cs:1) - calculates rental and tour prices
    - [`BLL.Interfaces.IDepositService`](BLL/Interfaces/IDepositService.cs:1) / [`BLL.Services.DepositService`](BLL/Services/DepositService.cs:1) - calculates deposits based on bike type and customer history
    - [`BLL.Interfaces.IMaintenanceService`](BLL/Interfaces/IMaintenanceService.cs:1) / [`BLL.Services.MaintenanceService`](BLL/Services/MaintenanceService.cs:1) - tracks maintenance needs and records

- **DAL**: Data access layer (EF Core) with complete implementation.
  - DbContext: [`DAL.AppDbContext`](DAL/AppDbContext.cs:5) - with all DbSets and relationships configured
  - EF design-time factory (migrations): [`DAL.AppDbContextFactory.CreateDbContext()`](DAL/AppDbContextFactory.cs:8)
  - Repository: [`DAL.Repository`](DAL/Repository.cs:1) - generic repository implementation
  - Migrations: [`DAL.Migrations`](DAL/Migrations/20260116091921_InitialCreate.cs:1) - initial schema creation

## Intended layering and responsibilities
- **Webapp**
  - Handles HTTP + Razor Pages + UI validation.
  - Calls into BLL services to perform operations.
  - Uses repository pattern for data access.
  - Implements responsive UI with Bootstrap 5.

- **BLL**
  - Domain model + business rules:
    - availability/overlap detection (IAvailabilityService)
    - rental extension rules (IAvailabilityService.IsRentalExtendableAsync)
    - tour capacity rules (ITourService - implicit in Webapp)
    - pricing calculation (4h blocks / full-day) (IPricingService)
    - deposit calculation (bike type + customer damage history) (IDepositService)
    - maintenance flagging (threshold - buffer) (IMaintenanceService)

- **DAL**
  - EF Core persistence, mapping, migrations.
  - SQLite is the default target database.
  - Repository pattern implementation for data access.

## Key technical decisions (current state)
- EF Core with SQLite backend (configurable via appsettings)
- Generic repository pattern with DI injection
- Service layer abstraction with DI
- Razor Pages for UI with Bootstrap styling
- Migrations and database initialization implemented

## Critical implementation paths (completed)
1. **Availability checking** - Implemented in AvailabilityService
2. **Return workflow** - Implemented in Rentals/Return.cshtml.cs
3. **Tour booking workflow** - Implemented in Tours/Book.cshtml.cs
4. **Pricing calculation** - Implemented in PricingService
5. **Deposit calculation** - Implemented in DepositService
6. **Maintenance tracking** - Implemented in MaintenanceService

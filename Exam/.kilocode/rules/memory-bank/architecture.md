# Architecture

## Solution structure
The solution is split into three projects (see [`Exam.sln`](Exam.sln)):

- **Webapp**: ASP.NET Core Razor Pages UI layer.
  - Entry point: [`Webapp.Program`](Webapp/Program.cs:1)
  - Pages: 
    - Bikes: [`Create`](Webapp/Pages/Bikes/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Bikes/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Bikes/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Bikes/Delete.cshtml.cs:1)
    - Customers: [`Create`](Webapp/Pages/Customers/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Customers/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Customers/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Customers/Delete.cshtml.cs:1)
    - Rentals: [`Create`](Webapp/Pages/Rentals/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Rentals/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Rentals/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Rentals/Delete.cshtml.cs:1), [`Extend`](Webapp/Pages/Rentals/Extend.cshtml.cs:1), [`Return`](Webapp/Pages/Rentals/Return.cshtml.cs:1)
    - Reservations: [`Create`](Webapp/Pages/Reservations/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Reservations/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Reservations/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Reservations/Delete.cshtml.cs:1)
    - Tours: [`Create`](Webapp/Pages/Tours/Create.cshtml.cs:1), [`Edit`](Webapp/Pages/Tours/Edit.cshtml.cs:1), [`Index`](Webapp/Pages/Tours/Index.cshtml.cs:1), [`Delete`](Webapp/Pages/Tours/Delete.cshtml.cs:1), [`Book`](Webapp/Pages/Tours/Book.cshtml.cs:1)
  - Dashboard: [`Index.cshtml`](Webapp/Pages/Index.cshtml:1) with fleet statistics, maintenance alerts, upcoming tours
  - UI Features: Toast notifications, loading spinners, dark mode toggle, keyboard shortcuts in [`site.js`](Webapp/wwwroot/js/site.js:1) and [`site.css`](Webapp/wwwroot/css/site.css:1)

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

  - Enums (status tracking):
    - [`BLL.Enums.BikeStatus`](BLL/Enums/BikeStatus.cs:1) - Available, Rented, InMaintenance, Retired
    - [`BLL.Enums.RentalStatus`](BLL/Enums/RentalStatus.cs:1) - Active, Completed, Cancelled, Extended
    - [`BLL.Enums.RentalType`](BLL/Enums/RentalType.cs:1) - FourHour, FullDay
    - [`BLL.Enums.TourBookingStatus`](BLL/Enums/TourBookingStatus.cs:1) - Confirmed, Cancelled, Completed
    - [`BLL.Enums.ServiceType`](BLL/Enums/ServiceType.cs:1) - Routine, Repair, Inspection

  - Services (interfaces + implementations):
    - [`BLL.Interfaces.IAvailabilityService`](BLL/Interfaces/IAvailabilityService.cs:1) / [`BLL.Services.AvailabilityService`](BLL/Services/AvailabilityService.cs:1) - checks bike availability and rental conflicts
    - [`BLL.Interfaces.IPricingService`](BLL/Interfaces/IPricingService.cs:1) / [`BLL.Services.PricingService`](BLL/Services/PricingService.cs:1) - calculates rental and tour prices
    - [`BLL.Interfaces.IDepositService`](BLL/Interfaces/IDepositService.cs:1) / [`BLL.Services.DepositService`](BLL/Services/DepositService.cs:1) - calculates deposits based on bike type and customer history
    - [`BLL.Interfaces.IMaintenanceService`](BLL/Interfaces/IMaintenanceService.cs:1) / [`BLL.Services.MaintenanceService`](BLL/Services/MaintenanceService.cs:1) - tracks maintenance needs and records
    - [`BLL.Interfaces.ITourService`](BLL/Interfaces/ITourService.cs:1) / [`BLL.Services.TourService`](BLL/Services/TourService.cs:1) - manages tour capacity, bookings, and participant rentals

  - Validators (FluentValidation):
    - [`BLL.Validators.BikeValidator`](BLL/Validators/BikeValidator.cs:1) - bike number format, pricing, odometer rules
    - [`BLL.Validators.CustomerValidator`](BLL/Validators/CustomerValidator.cs:1) - email, phone, name validation
    - [`BLL.Validators.RentalValidator`](BLL/Validators/RentalValidator.cs:1) - date/time logic, pricing validation
    - [`BLL.Validators.TourBookingValidator`](BLL/Validators/TourBookingValidator.cs:1) - participant count, pricing validation

- **DAL**: Data access layer (EF Core) with complete implementation.
  - DbContext: [`DAL.AppDbContext`](DAL/AppDbContext.cs:5) - with all DbSets and relationships configured
  - EF design-time factory (migrations): [`DAL.AppDbContextFactory.CreateDbContext()`](DAL/AppDbContextFactory.cs:8)
  - Generic repository: [`DAL.Repository`](DAL/Repository.cs:1) - base repository implementation
  - Specialized repositories:
    - [`DAL.Repositories.BikeRepository`](DAL/Repositories/BikeRepository.cs:1) - search by number/type, filter by status/maintenance
    - [`DAL.Repositories.CustomerRepository`](DAL/Repositories/CustomerRepository.cs:1) - search by name/email/phone
    - [`DAL.Repositories.RentalRepository`](DAL/Repositories/RentalRepository.cs:1) - filter by status/type/date range
  - Database seeding: [`DAL.DbInitializer`](DAL/DbInitializer.cs:1) - seeds 80 bikes, 10 customers, 3 tours
  - Migrations: 
    - [`InitialCreate`](DAL/Migrations/20260116091921_InitialCreate.cs:1) - initial schema
    - [`SchemaComplianceUpdate`](DAL/Migrations/20260116134928_SchemaComplianceUpdate.cs:1) - Requirements.MD compliance

## Intended layering and responsibilities
- **Webapp**
  - Handles HTTP + Razor Pages + UI validation.
  - Calls into BLL services to perform operations.
  - Uses repository pattern for data access.
  - Implements responsive UI with Bootstrap 5 and Bootstrap Icons.
  - Provides search/filter capabilities on all index pages.
  - Dashboard with real-time statistics and alerts.

- **BLL**
  - Domain model + business rules:
    - availability/overlap detection (IAvailabilityService)
    - rental extension rules (IAvailabilityService.IsRentalExtendableAsync)
    - tour capacity rules (ITourService.GetAvailableCapacityAsync)
    - pricing calculation (4h blocks / full-day) (IPricingService)
    - deposit calculation (bike type + customer damage history) (IDepositService)
    - maintenance flagging (threshold - buffer) (IMaintenanceService)
  - FluentValidation for comprehensive input validation.
  - Enum types for status tracking and type safety.

- **DAL**
  - EF Core persistence, mapping, migrations.
  - SQLite is the default target database.
  - Repository pattern implementation for data access.
  - Specialized repositories with search/filter capabilities.
  - Database initialization with seed data.

## Key technical decisions (current state)
- EF Core with SQLite backend (configurable via appsettings)
- Generic + specialized repository pattern with DI injection
- Service layer abstraction with DI
- FluentValidation.AspNetCore 11.3.0 for validation
- Razor Pages for UI with Bootstrap 5 styling and Bootstrap Icons
- DateOnly/TimeOnly for date/time separation (Requirements.MD compliance)
- Enum types for status tracking and type safety
- Migrations implemented (2 migrations: InitialCreate, SchemaComplianceUpdate)
- TestConsole project for testing services and database initialization
- Dark mode support with CSS variables
- Toast notifications and loading spinners for better UX
- Keyboard shortcuts for power users

## Critical implementation paths (completed)
1. **Availability checking** - Implemented in [`BLL.Services.AvailabilityService`](BLL/Services/AvailabilityService.cs:1)
2. **Return workflow** - Implemented in [`Webapp.Pages.Rentals.Return`](Webapp/Pages/Rentals/Return.cshtml.cs:1)
3. **Tour booking workflow** - Implemented in [`Webapp.Pages.Tours.Book`](Webapp/Pages/Tours/Book.cshtml.cs:1)
4. **Pricing calculation** - Implemented in [`BLL.Services.PricingService`](BLL/Services/PricingService.cs:1)
5. **Deposit calculation** - Implemented in [`BLL.Services.DepositService`](BLL/Services/DepositService.cs:1)
6. **Maintenance tracking** - Implemented in [`BLL.Services.MaintenanceService`](BLL/Services/MaintenanceService.cs:1)
7. **Tour capacity management** - Implemented in [`BLL.Services.TourService`](BLL/Services/TourService.cs:1)
8. **Search/filter functionality** - Implemented in specialized repositories and UI pages
9. **Dashboard statistics** - Implemented in [`Webapp.Pages.Index`](Webapp/Pages/Index.cshtml.cs:1)

## Known gaps
**NONE** - All previously identified gaps have been resolved:
- ✅ ITourService interface and TourService implementation created
- ✅ DbInitializer class implemented with comprehensive seed data
- ✅ FluentValidation validators implemented (4 validators)
- ✅ ITourService registered in DI container
- ✅ Specialized repositories implemented with search/filter
- ✅ All business logic bugs fixed
- ✅ UI polish complete with professional features

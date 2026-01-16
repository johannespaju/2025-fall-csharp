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
- Run test console:
  - `dotnet run --project TestConsole`

## EF Core tooling notes
- Migrations use the design-time factory [`DAL.AppDbContextFactory.CreateDbContext()`](DAL/AppDbContextFactory.cs:8).
- SQLite DB path used by tooling:
  - `${USERPROFILE}/exam/app.db` (Windows example: `C:\Users\\<user>\\exam\\app.db`).

## Repo-wide constraints
- Nullability is enabled and several warnings are treated as errors (see [`Directory.Build.props`](Directory.Build.props:1)).

## Services and Features Implemented

### Availability Service
- **Location**: [`BLL.Services.AvailabilityService`](BLL/Services/AvailabilityService.cs:1)
- **Functionality**:
  - Checks bike availability for specific time windows
  - Detects overlapping rentals
  - Validates rental extensions
  - Prevents double-booking
  - Checks maintenance status before rental

### Pricing Service
- **Location**: [`BLL.Services.PricingService`](BLL/Services/PricingService.cs:1)
- **Functionality**:
  - Calculates rental prices based on duration (4-hour blocks or full-day)
  - Handles tour pricing per participant
  - Calculates upgrade fees for bike type changes
  - Implements flexible pricing logic

### Deposit Service
- **Location**: [`BLL.Services.DepositService`](BLL/Services/DepositService.cs:1)
- **Functionality**:
  - Calculates deposits based on bike type (€50 regular, €150 electric)
  - Adds surcharges for customer damage history (€10 per damage incident)
  - Manages base deposit rates for different bike categories

### Maintenance Service
- **Location**: [`BLL.Services.MaintenanceService`](BLL/Services/MaintenanceService.cs:1)
- **Functionality**:
  - Tracks maintenance needs based on odometer readings
  - Proactive maintenance flagging with 50km buffer
  - Records maintenance history and costs
  - Flags bikes for service
  - Manages bike availability during maintenance

## Database Design

### Entities and Relationships
- **Bikes**: 5 types (City, Electric, Mountain, Tandem, Children's) with pricing and odometer tracking
- **Rentals**: Time-based rental records with bike and customer associations
- **Customers**: Customer information and rental history
- **Tours**: Guided tour offerings with capacity limits
- **TourBookings**: Tour reservations with participant counts
- **MaintenanceRecords**: Bike maintenance history
- **DamageRecords**: Customer damage history

## UI Features

### Rental Management
- Create rentals with bike and customer selection
- Real-time price and deposit calculation
- Availability checking
- Rental extension functionality
- Return and damage reporting

### Tour Management
- Tour creation and booking
- Capacity management
- Automatic rental generation for tour participants
- Price calculation per participant

### Bike Management
- Bike inventory tracking
- Odometer and maintenance status monitoring
- Maintenance scheduling
- Bike availability management

### Customer Management
- Customer database
- Rental and damage history tracking
- Deposit calculation based on history

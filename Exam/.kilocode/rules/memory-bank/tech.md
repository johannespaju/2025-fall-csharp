# Tech

## Stack
- **Language/runtime**: C# with .NET target frameworks set to `net10.0` (see [`Webapp/Webapp.csproj`](Webapp/Webapp.csproj:1), [`DAL/DAL.csproj`](DAL/DAL.csproj:1), [`BLL/BLL.csproj`](BLL/BLL.csproj:1)).
- **Web**: ASP.NET Core Razor Pages (see [`Webapp.Program`](Webapp/Program.cs:1)).
- **ORM**: Entity Framework Core 10.0.2 (see [`DAL/DAL.csproj`](DAL/DAL.csproj:1)).
- **Database**: SQLite intended (design-time factory uses `UseSqlite`), with SQL Server provider referenced as well.
- **Validation**: FluentValidation.AspNetCore 11.3.0 for comprehensive input validation (see [`BLL/BLL.csproj`](BLL/BLL.csproj:1)).
- **Frontend**: Bootstrap 5 responsive design + Bootstrap Icons + jQuery (see [`Webapp/Pages/Shared/_Layout.cshtml`](Webapp/Pages/Shared/_Layout.cshtml:1)).
- **UI Features**: Dark mode with CSS variables, toast notifications, loading spinners, keyboard shortcuts.

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

### Tour Service
- **Location**: [`BLL.Services.TourService`](BLL/Services/TourService.cs:1)
- **Functionality**:
  - Checks tour capacity and availability
  - Creates tour bookings with participant management
  - Generates rental records for each tour participant
  - Validates tour capacity limits

## Validation Layer

### FluentValidation Integration
- **Package**: FluentValidation.AspNetCore 11.3.0
- **Registration**: Validators registered in [`Webapp.Program`](Webapp/Program.cs:1) with `AddValidatorsFromAssemblyContaining`
- **Validators**:
  - [`BikeValidator`](BLL/Validators/BikeValidator.cs:1) - bike number format (B-XXX), pricing rules, odometer validation
  - [`CustomerValidator`](BLL/Validators/CustomerValidator.cs:1) - email format, phone format, name length validation
  - [`RentalValidator`](BLL/Validators/RentalValidator.cs:1) - date/time logic, pricing validation, status rules
  - [`TourBookingValidator`](BLL/Validators/TourBookingValidator.cs:1) - participant count, pricing validation, status rules

## Database Design

### Entities and Relationships
- **Bikes**: 5 types (City, Electric, Mountain, Tandem, Children's) with pricing and odometer tracking
- **Rentals**: Time-based rental records with bike and customer associations
- **Customers**: Customer information and rental history
- **Tours**: Guided tour offerings with capacity limits
- **TourBookings**: Tour reservations with participant counts
- **MaintenanceRecords**: Bike maintenance history
- **DamageRecords**: Customer damage history

### Enum Types
- **BikeStatus**: Available, Rented, InMaintenance, Retired
- **RentalStatus**: Active, Completed, Cancelled, Extended
- **RentalType**: FourHour, FullDay
- **TourBookingStatus**: Confirmed, Cancelled, Completed
- **ServiceType**: Routine, Repair, Inspection

### Repository Pattern
- **Generic Repository**: [`DAL.Repository`](DAL/Repository.cs:1) - base CRUD operations
- **Specialized Repositories**:
  - [`BikeRepository`](DAL/Repositories/BikeRepository.cs:1) - search by number/type, filter by status/maintenance
  - [`CustomerRepository`](DAL/Repositories/CustomerRepository.cs:1) - search by name/email/phone
  - [`RentalRepository`](DAL/Repositories/RentalRepository.cs:1) - filter by status/type/date range

### Database Seeding
- **Location**: [`DAL.DbInitializer`](DAL/DbInitializer.cs:1)
- **Seed Data**:
  - 80 bikes (16 per type) with realistic odometer values
  - 10 customers with varied profiles
  - 3 tours (City, Park, Coastal) with different capacities and pricing

## UI Features

### Rental Management
- Create rentals with bike and customer selection
- Real-time price and deposit calculation
- Availability checking
- Rental extension functionality
- Return and damage reporting
- Search/filter on rentals index page

### Tour Management
- Tour creation and booking
- Capacity management
- Automatic rental generation for tour participants
- Price calculation per participant
- Search/filter on tours index page

### Bike Management
- Bike inventory tracking
- Odometer and maintenance status monitoring
- Maintenance scheduling
- Bike availability management
- Search/filter on bikes index page (by number, type, status)

### Customer Management
- Customer database
- Rental and damage history tracking
- Deposit calculation based on history
- Search/filter on customers index page (by name, email, phone)

### Dashboard Features
- **Location**: [`Webapp/Pages/Index.cshtml`](Webapp/Pages/Index.cshtml:1)
- Fleet statistics (total bikes, available, in maintenance, rented)
- Maintenance alerts for bikes needing service
- Upcoming tours display
- Real-time data updates

### UI Enhancements
- **Toast Notifications**: Success/error messages with auto-dismiss
- **Loading Spinners**: Visual feedback for async operations
- **Dark Mode**: Toggle with CSS variables for theme switching
- **Keyboard Shortcuts**:
  - Ctrl+K: Focus search
  - Ctrl+N: Create new item
  - Ctrl+D: Toggle dark mode
- **Bootstrap Icons**: Professional iconography throughout
- **Responsive Design**: Mobile-first approach with Bootstrap 5
- **JavaScript Features**: Implemented in [`site.js`](Webapp/wwwroot/js/site.js:1)
- **CSS Styling**: Custom styles and dark mode in [`site.css`](Webapp/wwwroot/css/site.css:1)

# Bicycle Rental Management System

Full-featured ASP.NET Core web application for managing bicycle rentals, guided tours, and fleet maintenance.

## 🎯 Features

### Core Functionality
- **Fleet Management**: 80-bike inventory across 5 categories (City, Electric, Mountain, Tandem, Children's)
- **Rental System**: 4-hour blocks and full-day rentals with real-time availability checking
- **Guided Tours**: 3 tour types with capacity management and automatic rental generation
- **Predictive Maintenance**: 50km proactive buffer before service thresholds
- **Dynamic Deposits**: €50-€150 based on bike type + customer damage history
- **Damage Tracking**: Complete incident history with deposit adjustments

### Business Rules
- **Rental Periods**: 09:00-13:00, 13:00-17:00, 17:00-21:00 (4-hour), 09:00-21:00 (full day)
- **Pricing**: €8-€28 per day depending on bike type
- **Maintenance Intervals**: City 500km, Electric 300km, Mountain 400km (flagged 50km early)
- **Tour Capacity**: 6-12 participants per tour with overflow prevention
- **Deposits**: Base €50/€150 + €10 per past damage incident

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 10.0 with Razor Pages
- **ORM**: Entity Framework Core 10.0.2
- **Database**: SQLite (production-ready, file-based)
- **Frontend**: Bootstrap 5 with responsive design
- **Validation**: FluentValidation for complex business rules
- **Architecture**: Layered (BLL → DAL → Webapp) with repository pattern

### Installation

1. **Clone repository**
   ```bash
   git clone <repo-url>
   cd Exam
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Run migrations**
   ```bash
   dotnet ef migrations add InitialCreate --project DAL --startup-project Webapp
   dotnet ef database update --project DAL --startup-project Webapp
   ```

4. **Run application**
   ```bash
   dotnet run --project Webapp
   ```

5. **Navigate to** https://localhost:5001

## 📐 Architecture

```
Exam/
├── BLL/                    # Business Logic Layer
│   ├── Entities/          # Domain models (Bike, Rental, Customer, Tour, etc.)
│   ├── Enums/             # BikeStatus, RentalStatus, etc.
│   ├── Services/          # Business logic (Availability, Pricing, Deposit, Maintenance)
│   ├── Interfaces/        # Service and repository contracts
│   └── Validators/        # FluentValidation rules
│
├── DAL/                    # Data Access Layer
│   ├── AppDbContext.cs    # EF Core DbContext
│   ├── Repository.cs      # Generic repository implementation
│   ├── DbInitializer.cs   # Seed data
│   └── Migrations/        # EF Core migrations
│
└── Webapp/                 # Presentation Layer
    ├── Pages/             # Razor Pages (Bikes, Rentals, Customers, Tours)
    ├── wwwroot/           # Static files (CSS, JS, images)
    └── Program.cs         # DI configuration and startup
```

## 🧪 Testing Scenarios

- ✅ Create bike → appears in filtered list
- ✅ Rent available bike → status updates to Rented
- ✅ Return bike at 450km (city bike) → auto-flags for maintenance
- ✅ Customer with 2 damages → deposit increases by €20
- ✅ Book tour for 3 → creates 1 TourBooking + 3 Rentals
- ✅ Book tour at capacity → rejected with error
- ✅ Extend rental with conflict → rejected
- ✅ Search bikes by type/status → correct results

## 📝 Key Implementation Details

### Maintenance Flagging
```csharp
// Bikes flagged 50km BEFORE threshold (proactive)
kmSinceService >= (serviceInterval - 50)
// Example: City bike flags at 450km, not 500km
```

### Tour Booking Process
1. Validate capacity available
2. Create TourBooking record
3. Get N available bikes (City or Electric if upgraded)
4. Create N Rental records (one per participant)
5. Link all rentals via TourBookingId
6. Set TourBooking.TotalCost = sum of rental costs

### Deposit Calculation
```csharp
deposit = (bikeType == Electric ? 150 : 50) + (damageCount × 10)
```

## 🎨 UI Features

- **Toast Notifications**: Success/error feedback
- **Loading Spinners**: Async operation indicators
- **Delete Confirmations**: Prevent accidental deletions
- **Dark Mode**: Toggle with localStorage persistence
- **Responsive Design**: Mobile-first Bootstrap 5 layout
- **Search/Filter**: All entity lists filterable
- **Dashboard**: Statistics and quick actions

## 📚 API & Services

### Core Services
- **IAvailabilityService**: Bike availability checking and conflict detection
- **IPricingService**: Rental and tour cost calculation
- **IDepositService**: Dynamic deposit calculation
- **IMaintenanceService**: Predictive maintenance flagging
- **ITourService**: Tour capacity and booking management

## 🔒 Business Logic Validation

- **Date/Time Separation**: StartDate + StartTime (not combined DateTime)
- **Time Block Enforcement**: Only 09:00, 13:00, 17:00, 21:00 for 4-hour rentals
- **Overlap Detection**: Prevents double-booking bikes
- **Extension Validation**: Checks for future reservation conflicts
- **Capacity Limits**: Prevents tour overbooking across time slots

## 📊 Database Schema

**7 Core Entities**: Bike, Customer, Rental, Tour, TourBooking, MaintenanceRecord, DamageRecord

**Key Relationships**:
- Customer 1:N Rental N:1 Bike
- Tour 1:N TourBooking 1:N Rental (via TourBookingId FK)
- Bike 1:N MaintenanceRecord
- Bike/Customer/Rental 1:N DamageRecord
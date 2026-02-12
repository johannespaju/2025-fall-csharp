# Bicycle Rental System (Exam Project)

## Exam task

**Exam task text is in** [`task.md`](task.md:1).

## Overview

This repository contains an ASP.NET Core Razor Pages application for managing a bicycle rental business in a tourist city.

Main workflows:
- Bike fleet management (types, pricing, status, odometer)
- Rentals (4-hour blocks / full-day) with availability checks and overlap prevention
- Rental extensions with conflict detection
- Reservations (pre-booked rentals)
- Guided tours with capacity management and participant rentals
- Returns with damage reporting
- Predictive maintenance tracking and maintenance records
- Deposit calculation based on bike type and customer damage history

Solution file: [`Exam.sln`](Exam.sln:1)

## Tech stack

- .NET / C# (ASP.NET Core Razor Pages)
- Entity Framework Core + SQLite
- FluentValidation (server-side validation)
- Bootstrap 5 + Bootstrap Icons (UI)

Entry point / DI setup: [`Webapp.Program`](Webapp/Program.cs:1)

## Solution structure

- [`Webapp/`](Webapp:1) — Razor Pages UI (CRUD pages, dashboards, workflows)
- [`BLL/`](BLL:1) — domain entities, business services, validators
- [`DAL/`](DAL:1) — EF Core persistence (DbContext, repositories, migrations, seeding)
- [`TestConsole/`](TestConsole:1) — small console runner for testing services/DB initialization

Key files:
- DbContext: [`DAL.AppDbContext`](DAL/AppDbContext.cs:1)
- Design-time factory (migrations): [`DAL.AppDbContextFactory.CreateDbContext()`](DAL/AppDbContextFactory.cs:8)
- Seeding: [`DAL.DbInitializer`](DAL/DbInitializer.cs:1)

## Run locally

### Prerequisites

- .NET SDK (see target frameworks in [`Webapp/Webapp.csproj`](Webapp/Webapp.csproj:1))

### Restore + build

```bash
dotnet restore
dotnet build
```

### Run the web app

```bash
dotnet run --project Webapp
```

Open the URL printed in the console.

## Database notes (SQLite)

- Default database is SQLite.
- EF Core migrations live in [`DAL/Migrations/`](DAL/Migrations:1).
- Migrations use the design-time factory [`DAL.AppDbContextFactory.CreateDbContext()`](DAL/AppDbContextFactory.cs:8).
- Seed data is created by [`DAL.DbInitializer`](DAL/DbInitializer.cs:1) (bikes, customers, tours).

### Database file location

EF tooling / default setup uses:
- `%USERPROFILE%\exam\app.db` (Windows example: `C:\Users\<user>\exam\app.db`)

### Reset the database

1. Stop the app.
2. Delete the SQLite file: `%USERPROFILE%\exam\app.db`.
3. Start the app again (seeding will recreate data if enabled).

### (Optional) Apply migrations manually

```bash
dotnet ef database update --project DAL --startup-project Webapp
```

## (Optional) Run TestConsole

```bash
dotnet run --project TestConsole
```

Console entry point: [`TestConsole.Program`](TestConsole/Program.cs:1)

# icd0008 – C# and .NET Projects

**Johannes Paju** | TalTech

This repository contains two full-stack C# / .NET projects built during the icd0008 course.

---

## [HyperConnectX](hyper-connectx/) — Customizable Connect Four Game

A Connect Four-style game with a shared engine powering both a console UI and a web UI.

- **Tech:** .NET 10, ASP.NET Core Razor Pages, Entity Framework Core, SQLite
- **Highlights:**
  - Configurable board size (3–20), win condition, and optional cylindrical board
  - AI opponent (Minimax with alpha-beta pruning) with Easy / Medium / Hard difficulty
  - Player vs Player, Player vs Computer, and Computer vs Computer modes
  - Switchable persistence: SQLite (EF Core) or JSON files
  - Save / load games and rule configurations

## [Bicycle Rental System](Exam/) — Exam Project

An ASP.NET Core Razor Pages application for managing a bicycle rental business.

- **Tech:** .NET, ASP.NET Core Razor Pages, Entity Framework Core, SQLite, FluentValidation
- **Highlights:**
  - Bike fleet management with types, pricing, status, and odometer tracking
  - Rentals (4-hour blocks / full-day) with availability checks and overlap prevention
  - Reservations, rental extensions with conflict detection
  - Guided tours with capacity management
  - Returns with damage reporting
  - Predictive maintenance tracking
  - Deposit calculation based on bike type and customer damage history

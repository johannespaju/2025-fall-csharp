# Bicycle Rental System - Project Overview

## 🎯 Main Objectives

Develop a full-featured **ASP.NET Core** web application for managing bicycle rentals, guided tours, and fleet maintenance in a tourist city environment. The system handles **80 bikes** across 5 categories with complex business logic for availability, pricing, maintenance scheduling, and tour capacity management.

## ✨ Key Features

- **Multi-tier bike inventory management** (80 bikes: city, electric, mountain, tandem, children's)
- **Flexible rental system** with 4-hour blocks and full-day options (€8-€28/day rates)
- **Guided tour scheduling** with three route types and capacity management (6-12 participants)
- **Predictive maintenance tracking** based on bike-specific odometer intervals (300-500km)
- **Real-time availability checking** with conflict detection for rental extensions
- **Dynamic deposit calculation** (€50-€150) based on bike type and customer damage history
- **Automated maintenance flagging** with 50km proactive buffer before service thresholds
- **Tour booking system** that auto-generates rental records for each participant

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 10.0 with Razor Pages
- **ORM**: Entity Framework Core with SQLite database
- **Frontend**: Bootstrap 5 responsive design with mobile-first approach
- **Validation**: FluentValidation for complex business rules
- **Architecture**: Repository pattern, service layer for business logic, separation of concerns

## 🧮 Core Business Logic

- **Availability algorithm** checking bike status and overlapping rental periods
- **Maintenance flagging** when bikes reach threshold minus 50km buffer (proactive scheduling)
- **Tour bookings** create one TourBooking record plus N Rental records (one per participant)
- **Deposit calculation**: Base amount (€50/€150) + €10 per historical damage incident
- **Extension validation** preventing conflicts with future reservations
- **Capacity management** preventing tour overbooking across multiple time slots

## ⏱️ Development Context

**8-hour exam project** requiring MVP-first approach with vertical slices. Implementation prioritized in four tiers:

1. Core CRUD operations
2. Business logic services
3. Full feature set with tours and extensions
4. UX polish with responsive design and notifications

## 🎓 Significance

Demonstrates comprehensive full-stack development skills including:

- Complex domain modeling with multiple entity relationships
- Repository and service layer architectural patterns
- Real-world business rule implementation and validation
- Asynchronous database operations and N+1 query prevention
- Responsive UI design with accessibility considerations
- Professional software engineering practices under time constraints

## 📋 Project Scope

Educational examination project showcasing ability to deliver production-quality code with proper separation of concerns, comprehensive validation, and user-friendly interfaces within strict time limits.
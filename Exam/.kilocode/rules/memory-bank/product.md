# Product

## Why this project exists
This is an exam project to demonstrate building a production-style ASP.NET Core web app (Razor Pages) with a clean layered architecture for a bicycle rental & guided tour business.

## Problems it solves
- Keep track of an 80-bike fleet across multiple categories with different pricing.
- Manage rental periods (4-hour blocks or full-day) and ensure availability (no overlapping rentals).
- Support guided tours with fixed start times and capacity limits.
- Track odometer-based maintenance schedules, proactively flag bikes that need servicing, and track maintenance cost.
- Handle rental extensions by checking for conflicts with future reservations.
- Manage deposits and adjust deposit levels based on customer/bike damage history.

## How it should work (user-facing behavior)
- Staff can create customers, bikes, rentals, and tour bookings.
- When creating rentals or tours, the system checks bike availability for the requested time window.
- Tours create bookings with participant count and include bike rental; users can upgrade bike type for an additional fee.
- When a bike is returned, staff records odometer and any damage; maintenance/damage history affects future availability and deposits.
- Deposits are computed based on bike type (regular vs electric) and customer damage history.

## User experience goals
- Fast workflows for common operations (rent-out, return, extend, book tour).
- Clear availability feedback (why a bike cannot be rented/extended).
- Admin-friendly overview dashboards (fleet status, bikes due for maintenance, upcoming tours).
- Low-risk data entry (validation, sensible defaults, minimal required fields).


# FixItNow

**A full-stack service marketplace platform connecting customers with local technicians for on-demand home services.**

Built with .NET 8, Blazor Server, PostgreSQL, and SignalR — deployed on Microsoft Azure.

🌐 **Live Demo:** [fixitnow-api-ernesto-g7ffa2bcdaejeyd0.southeastasia-01.azurewebsites.net](https://fixitnow-api-ernesto-g7ffa2bcdaejeyd0.southeastasia-01.azurewebsites.net/dashboard)

---

## Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| Customer | customer@fixitnow.demo | Demo123! |
| Technician | technician@fixitnow.demo | Demo123! |
| Admin | admin@fixitnow.demo | Demo123! |

---

## What It Does

FixItNow is inspired by platforms like Gojek and TaskRabbit. Customers can browse verified technicians, send booking requests, track service progress, and chat in real time. Technicians manage their availability, accept or decline jobs, and build a profile with reviews.

Payment is handled outside the app (cash, GoPay, bank transfer) — the platform focuses on discovery, booking, job tracking, communication, and trust.

---

## Features

**Authentication & Authorization**
- JWT-based login and registration
- Role-based access control (Customer, Technician, Admin)
- Protected API endpoints and secure SignalR connections

**Technician System**
- Apply to become a technician with profile details
- Admin approval/rejection flow with rejection reasons
- Online/offline availability toggle
- Profile images uploaded to Azure Blob Storage

**Booking Requests**
- Customers send booking requests to specific technicians
- Technicians accept or decline with real-time notifications
- Cancellation flow for both sides

**Ticket Management**
- Service tickets created from accepted bookings
- Structured lifecycle: `Unassigned → Assigned → In Progress → Completed`
- Image attachments per ticket

**Real-Time Chat**
- SignalR-powered messaging per ticket
- Full conversation history with timestamps
- Secure participant validation

**Reviews**
- Customers leave ratings (1–5) and comments after job completion
- Per-technician review aggregation with average rating

**Notifications**
- Hybrid system: persisted to PostgreSQL + real-time delivery via SignalR
- Unread count badge and notification dropdown in the top bar
- Triggered by booking updates, approvals, rejections

**Admin Dashboard**
- Review pending technician applications
- Approve or reject with a reason
- Rejected technicians can re-apply

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core Web API (.NET 8) |
| Frontend | Blazor Server, MudBlazor |
| Real-Time | SignalR |
| Database | PostgreSQL (Azure) |
| ORM | Entity Framework Core |
| Auth | JWT |
| Storage | Azure Blob Storage |
| Hosting | Azure App Service |

---

## Architecture

Clean Architecture with four projects:

```
FixItNow.Web            → Controllers, SignalR Hubs, API Services, Blazor UI
FixItNow.Application    → Business Logic, Services, Validators
FixItNow.Domain         → Entities, DTOs, Enums
FixItNow.Infrastructure → EF Core, Migrations, DbContext, DI Registration
```

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL

### Run Locally

```bash
git clone https://github.com/ernesto-04/FixItNow.git
cd FixItNow
dotnet run --project FixItNow.Web
```

The app auto-migrates the database on startup.

### Configuration

Set your connection string and JWT secret in `appsettings.json` or via environment variables:

```json
{
  "ConnectionStrings": {
    "Default": "Host=...;Database=fixitnow;Username=...;Password=..."
  },
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "FixItNow",
    "Audience": "FixItNow"
  }
}
```

---

## Project Highlights

- **Real-world workflow** — full booking-to-completion lifecycle with state machine ticket transitions
- **Hybrid notification system** — every notification persisted to DB and delivered live via SignalR
- **Clean Architecture** — strict layer separation, DI throughout, no business logic in controllers
- **Auth prerender handling** — custom `AuthenticationStateProvider` that safely handles JS interop exceptions during Blazor Server prerender
- **Azure-deployed** — live on Azure App Service with Azure PostgreSQL and Blob Storage

---

## Disclaimer

Built for educational and portfolio purposes. All demo data is synthetic and does not represent real users or organizations.

---

## Author

Developed by [ernesto-04](https://github.com/ernesto-04)

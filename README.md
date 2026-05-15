# FixItNow – Service Marketplace Platform

## 📌 Overview

FixItNow is a full-stack web-based service marketplace platform that connects customers with technicians for on-demand issue resolution.

The platform allows customers to create service requests, while technicians can browse, accept, manage, and complete tasks through a structured workflow. The system also supports real-time communication between customers and technicians using SignalR.

This project focuses on clean architecture, scalable backend design, real-world workflow implementation, and cloud deployment using Microsoft Azure.

🌐 Live Demo: https://your-azure-app.azurewebsites.net

---

## 🧠 Problem Statement

In Indonesia, finding reliable technicians (e.g., for home repairs or maintenance) is often difficult.

Most people rely on:

* Personal recommendations from friends or family
* Informal networks with limited availability
* Unverified service providers

This leads to:

* Delays in getting help
* Lack of transparency in service quality
* Inefficient communication between customers and technicians

There is a need for a centralized platform to:

* Connect customers with available technicians
* Standardize the service request process
* Improve visibility and tracking of tasks
* Provide a more structured communication workflow

---

## 🏗️ Architecture

This project follows **Clean Architecture principles**, ensuring maintainability, scalability, and separation of concerns.

```text
/Presentation Layer   → Blazor WebAssembly UI
/Application Layer    → Business logic & use cases
/Domain Layer         → Core entities & models
/Infrastructure Layer → Database & external services
```

### Key Design Decisions

* Clean separation between layers
* Dependency Injection for service management
* Service abstraction for maintainability
* Async-first architecture for scalability
* Role-based authorization for secure access
* Modular feature organization
* Environment-based configuration for deployment

---

## ⚙️ Tech Stack

### Backend
* ASP.NET Core Web API (.NET 8)
* Entity Framework Core
* SignalR

### Frontend
* Blazor WebAssembly
* MudBlazor

### Database
* PostgreSQL

### Authentication & Security
* JWT Authentication
* Role-Based Authorization

### Cloud & DevOps
* Microsoft Azure App Service
* Azure PostgreSQL Flexible Server

---

## 🔄 Core Features

### 🔐 Authentication & Authorization

* Secure JWT-based authentication
* Role-based access control (Customer & Technician)
* Protected API endpoints
* Secure SignalR communication

---

### 🧾 Ticket Management

* Customers can create service requests
* Includes title, description, category, and location
* Ticket status tracking
* Structured ticket workflow lifecycle

---

### 👨‍🔧 Technician Workflow

* Browse available (unassigned) tickets
* Accept and manage assigned tickets
* Update work progress
* Complete assigned tasks

---

### 💬 Real-Time Chat System

* Real-time messaging using SignalR
* Separate chat interface for customers and technicians
* Ticket-based communication rooms
* Conversation history & timestamps
* Secure access validation for chat participants

---

### 🔁 Ticket Lifecycle

```text
Unassigned → Assigned → In Progress → Completed
```

---

## 📊 System Workflow

1. Customer logs in and creates a service ticket
2. Ticket status becomes **Unassigned**
3. Technician browses available tickets
4. Technician accepts ticket → **Assigned**
5. Technician updates progress → **In Progress**
6. Customer and technician communicate in real time
7. Task completed → **Completed**

---

## 🚀 Live Deployment

FixItNow is deployed publicly using Microsoft Azure.

### Cloud Infrastructure

* Azure App Service
* Azure PostgreSQL Database
* HTTPS-enabled deployment
* Environment-based configuration management

🌐 Live Website: https://your-azure-app.azurewebsites.net

---

## 🚀 Getting Started

### Prerequisites

* .NET 8 SDK
* PostgreSQL

---

### Run Locally

```bash
dotnet run
```

---

### Environment Configuration

The project supports multiple environments:

* Development → Local PostgreSQL
* Production → Azure PostgreSQL

Environment variables are used for secure production configuration.

---

## 🧪 Demo Notes

* This project uses development/demo data
* No real customer data is included
* Built primarily for learning and portfolio purposes

---

## 🧩 Future Improvements

* Notification system (real-time & email)
* File/image attachments in chat
* Technician rating & review system
* Advanced filtering & search
* Pagination & performance optimization
* Redis caching
* Admin dashboard & analytics

---

## 📚 What This Project Demonstrates

* Clean Architecture implementation
* RESTful API development
* JWT authentication & authorization
* Real-time communication using SignalR
* Blazor WebAssembly frontend development
* PostgreSQL integration with Entity Framework Core
* Azure cloud deployment
* Role-based workflow management
* Real-world service marketplace modeling
* Scalable backend structure & maintainability

---

## ⚠️ Disclaimer

This project was built for educational and portfolio purposes. All data shown is synthetic and does not represent real users or organizations.

---

## 👤 Author

Developed by Ernesto as a personal full-stack portfolio project using modern .NET technologies.

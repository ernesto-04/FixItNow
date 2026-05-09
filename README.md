# FixItNow – Service Marketplace Platform

## 📌 Overview

FixItNow is a web-based service marketplace platform that connects customers with technicians for on-demand issue resolution.

The system enables customers to create service requests, while technicians can browse, accept, and complete tasks through a structured workflow. The project focuses on clean architecture, scalable backend design, and clear separation of concerns.

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


---

## 🏗️ Architecture

This project follows **Clean Architecture principles**, ensuring maintainability and scalability.

```
/Presentation Layer   → Blazor UI (Client-side)
/Application Layer    → Business logic & use cases
/Domain Layer         → Core entities & models
/Infrastructure Layer → Data access & external services
```

### Key Design Decisions

* Separation of concerns between layers
* Dependency injection for service management
* Service layer abstraction for business logic
* Async-first approach for scalability

---

## ⚙️ Tech Stack

* **Backend:** .NET 8 (C#)
* **Frontend:** Blazor WebAssembly
* **Database:** PostgreSQL (or In-Memory for demo)
* **Authentication:** JWT (JSON Web Token)
* **UI Library:** MudBlazor

---

## 🔄 Core Features

### 🔐 Authentication & Authorization

* Secure authentication using JWT
* Role-based access control (Customer & Technician)
* Protected API endpoints

---

### 🧾 Ticket Management

* Customers can create service requests
* Includes title, description, category, and location

---

### 👨‍🔧 Technician Workflow

* View available (unassigned) tickets
* Accept tickets
* Update ticket status

---

### 🔁 Ticket Lifecycle

```
Unassigned → Assigned → In Progress → Completed
```

---

## 📊 System Flow

1. Customer logs in and creates a ticket
2. Ticket is marked as **Unassigned**
3. Technician logs in and accepts the ticket → **Assigned**
4. Technician starts work → **In Progress**
5. Task completed → **Completed**

---

## 🚀 Getting Started

### Prerequisites

* .NET 8 SDK installed

### Run the project

```bash
dotnet run
```

The application will start locally and can be accessed via the browser.

---

## 🧪 Demo Notes

* This project uses **mock or development data** for demonstration
* No real user data is included

---

## 🧩 Future Improvements

* Distributed caching using Redis for performance optimization
* Real-time updates (SignalR)
* Notification system (email or in-app)
* Advanced filtering & search

---

## 📚 What This Project Demonstrates

* Clean Architecture implementation
* RESTful API design
* JWT-based authentication & authorization
* State management in Blazor
* Real-world workflow modeling
* Scalable service-layer design

---

## ⚠️ Disclaimer

This project is built for demonstration purposes. All data is synthetic and does not represent any real users or organizations.

---

## 👤 Author

Developed as part of a personal portfolio to demonstrate full-stack engineering capabilities using .NET technologies.

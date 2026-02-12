# SportDiary

SportDiary is an ASP.NET Core MVC web application for tracking personal training sessions and managing fitness activity history.

The project demonstrates layered architecture, ASP.NET Core Identity authentication, CRUD operations, validation, and clean MVC structure.

---

## 🚀 Features

- User registration and login (ASP.NET Core Identity)
- Personal user profiles
- Training diary management
- Training entries (Create, Edit, Delete, Details)
- "Only my data" security filtering
- Server-side and client-side validation
- Responsive UI with Bootstrap
- Clean layered architecture (Data / Services / Web)

---

## 🏗 Architecture

The solution follows a layered structure:

- **SportDiary (Web)** – Controllers, Views, UI
- **SportDiary.Data** – DbContext and Entity models
- **SportDiary.Services** – Business logic (Service layer)
- **SportDiary.ViewModels** – ViewModels used in forms and views
- **SportDiary.Common** – Validation constants and shared utilities

Controllers do not access the database directly.  
All business logic is handled through services using Dependency Injection.

---

## 🛠 Technologies Used

- ASP.NET Core (.NET 8)
- MVC Architecture
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Razor Views
- Bootstrap 5
- Git & GitHub

---

## 🔐 Authentication & Authorization

The application uses ASP.NET Core Identity.

- Users must register and log in
- Each user can access only their own training data
- Protected pages require authentication

---

## 🗄 Database Setup

1. Update the connection string in:


2. Open Package Manager Console and run:


3. Run the application.

---

## ▶ How to Run the Project

1. Clone the repository
2. Open `SportDiary.sln`
3. Restore NuGet packages
4. Apply migrations
5. Run the project

The application will start using the default configuration.

---

## 📌 Project Purpose

This project was developed as part of the ASP.NET Fundamentals course assignment.

It demonstrates understanding of:

- MVC pattern
- Identity integration
- Dependency Injection
- Layered architecture
- CRUD operations
- Data validation
- Secure user-based data access

---

## 👩‍💻 Author

PetqEva


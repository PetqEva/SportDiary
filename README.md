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
- **SportDiary.GCommon** – Validation constants and shared utilities

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

### Demo user (seed)

On local run, the app seeds a demo account + sample diary/entries (see `Infrastructure/DbSeeder.cs`).

- Email: `demo@sportdiary.local`
- Password: `demo123`

If you don't want seed data for submission, comment out the seeding line in `Program.cs`.

---

## 🗄 Database Setup

1. Configure SQL Server connection string in:

   - `SportDiary/appsettings.json` (or `SportDiary/appsettings.Development.json`)
   - Key: `ConnectionStrings:DefaultConnection`

   Example:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SportDiaryDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

2. Apply migrations:

   **Option A (Package Manager Console):**
   ```powershell
   Update-Database -Project SportDiary.Data -StartupProject SportDiary
   ```

   **Option B (CLI):**
   ```bash
   dotnet ef database update --project SportDiary.Data --startup-project SportDiary
   ```

3. Run the application.

---

## ▶ How to Run the Project

1. Clone the repository.
2. Open `SportDiary.sln`.
3. Restore NuGet packages.
4. Apply migrations (see **Database Setup**).
5. Run `SportDiary` (the Web project).

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


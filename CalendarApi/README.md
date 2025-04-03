# Calendar Event Management System

This solution is a Calendar Event Management System built with .NET 8.0 and C# 12.0. It provides functionality to create, update, delete, and retrieve calendar events using a RESTful API.

---

## Project Structure

- **Api**: Contains the API endpoints for managing events.
- **Application**: Contains the business logic and service layer.
- **Domain**: Contains the domain models.
- **Infrastructure**: Contains the database context and migrations.

---

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server

---

### Installation

1. Clone the repository:
- git clone https://github.com/SuperHetFrontend/awol-dev-test.git

2. Restore the dependencies:
- dotnet restore

3. Configure Database Connection if not using LocalDb
- Modify appsettings.Development.json inside the API project:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CalendarDevTest;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

For sql server update similar to...

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=CalendarDevTest;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

4. Update Database
- Successfully running the application will create the database and seed it using the existing migrations!
- Alternatively delete the migrations folder and start afresh using the ef commands
	- dotnet ef migrations add InitialCreate
	- dotnet ef database update

---

### Run the Application
- dotnet build
- dotnet run --project API

---

## Technologies Used

- .NET 8.0
- C# 12.0
- Entity Framework Core 8.0
- SQL Server LocalDB
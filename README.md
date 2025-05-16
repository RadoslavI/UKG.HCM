
# UKG HCM - Human Capital Management System

A .NET 8+ backend solution developed for the UKG BE HCM assignment. The project simulates a production-grade Human Capital Management (HCM) system suitable for HR operations in a small to medium-sized company.

## ✨ Features

- **People Management**  
  - Full CRUD for managing people records

- **Authentication & Authorization**  
  - Custom login system (no ASP.NET Identity)
  - Role-based access control:
    - `Employee`
    - `Manager`
    - `HR Admin`

- **Modular Architecture**  
  - Developed as two separate APIs:
    - `People.API`: Manages person records
    - `Auth.API`: Handles login and user management

- **Testing**  
  - Unit tests and integration tests implemented

## 🏗️ Technologies Used

- .NET 8
- C#
- Razor Pages (for optional UI)
- InMemory database (for local development/testing)
- NUnit (testing)
- Dependency Injection
- Minimal API configuration
- Clean architecture practices

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- IDE (Visual Studio, Rider, or VS Code)

### Run the Project

1. Clone the repository:

   ```bash
   git clone https://github.com/radoslavi/ukg.hcm.git
   cd ukg-hcm
   ```

2. Navigate to each API and run:

   ```bash
   cd src/Auth.API
   dotnet run
   ```

   ```bash
   cd src/People.API
   dotnet run
   ```

3. (Optional) Run the Razor Pages frontend:

   ```bash
   cd src/UI
   dotnet run
   ```

4. APIs should now be available at:
   - Auth API: `https://localhost:5001`
   - People API: `https://localhost:5002`

5. Check the appsettings if all endpoints are properly entered with the correct address

## 🧪 Running Tests

```bash
dotnet test
```

Test projects are located in the `/tests` directory.

## 🗂️ Project Structure

```
UKG.HCM/
├── src/
│   ├── Auth.API/        # Authentication & Authorization API
│   ├── People.API/      # People Management API
│   ├── UI/              # Optional Razor Pages UI
│
├── tests/
│   ├── Auth.Tests/
│   ├── People.Tests/
│
└── README.md
```

## 🔐 Roles and Access

| Role        | Permissions              |
|-------------|--------------------------|
| Employee    | View and edit own record |
| Manager     | View all records         |
| HR Admin    | Full CRUD on all people  |

## 📌 Notes

- No use of ASP.NET Identity (custom authentication implemented).
- Authentication is token-based.
- In-memory data store used for development.
- Designed with extensibility in mind.

## 📄 License

MIT License

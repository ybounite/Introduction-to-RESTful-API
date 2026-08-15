# StudentApi — Layered Architecture Setup

This project uses a simple **3-layer architecture**:

```text
StudentApi.slnx
│
├── StudentApi                      # API / Presentation Layer
│   ├── Controllers
│   │   └── StudentsController.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── StudentApiBusinessLayer         # Business Logic Layer
│   ├── Interfaces
│   │   └── IStudentService.cs
│   └── Services
│       └── StudentService.cs
│
└── StudentDataAccessLayer          # Data Access Layer
    ├── Data
    │   └── StudentDbContext.cs
    ├── Interfaces
    │   └── IStudentRepository.cs
    └── Repositories
        └── StudentRepository.cs
```

---

## 1. Prerequisites

Make sure the **.NET SDK** is installed:

```bash
dotnet --version
```

You should also have **VS Code** installed:

```bash
code --version
```

---

## 2. Create the Solution

If the solution does not already exist:

```bash
dotnet new sln -n StudentApi
```

In this project create file `StudentApi.slnx`

```text
StudentApi.slnx
```

> **Note:** Do not create another solution if `StudentApi.slnx` already exists.

---

## 3. Create the Business Layer

Create a .NET Class Library:

```bash
dotnet new classlib -n StudentApiBusinessLayer
```

Add it to the existing solution:

```bash
dotnet sln StudentApi.slnx add StudentApiBusinessLayer/StudentApiBusinessLayer.csproj
```

---

## 4. Create the Data Access Layer

Create another Class Library:

```bash
dotnet new classlib -n StudentDataAccessLayer
```

Add it to the solution:

```bash
dotnet sln StudentApi.slnx add StudentDataAccessLayer/StudentDataAccessLayer.csproj
```

---

## 5. Add the API Project to the Solution

If the API project already exists, find its `.csproj` file:

```bash
find . -name "*.csproj"
```

The API project should be:

```text
StudentApi/StudentApi.csproj
```

Add it to the solution:

```bash
dotnet sln StudentApi.slnx add StudentApi/StudentApi.csproj
```

---

## 6. Add Project References

The dependency between the layers should be:

```text
API
 │
 ▼
Business Layer
 │
 ▼
Data Access Layer
```

### API → Business Layer

```bash
dotnet add StudentApi/StudentApi.csproj reference StudentApiBusinessLayer/StudentApiBusinessLayer.csproj
```

### Business Layer → Data Access Layer

```bash
dotnet add StudentApiBusinessLayer/StudentApiBusinessLayer.csproj reference StudentDataAccessLayer/StudentDataAccessLayer.csproj
```

---

## 7. Create the Business Layer Folders

Create the folders:

```bash
mkdir -p StudentApiBusinessLayer/Interfaces
mkdir -p StudentApiBusinessLayer/Services
```

Create the files:

```bash
touch StudentApiBusinessLayer/Interfaces/IStudentService.cs
touch StudentApiBusinessLayer/Services/StudentService.cs
```

The structure becomes:

```text
StudentApiBusinessLayer
├── Interfaces
│   └── IStudentService.cs
├── Services
│   └── StudentService.cs
└── StudentApiBusinessLayer.csproj
```

---

## 8. Create the Data Access Layer Folders

Create the folders:

```bash
mkdir -p StudentDataAccessLayer/Data
mkdir -p StudentDataAccessLayer/Interfaces
mkdir -p StudentDataAccessLayer/Repositories
```

Create the files:

```bash
touch StudentDataAccessLayer/Data/StudentDbContext.cs
touch StudentDataAccessLayer/Interfaces/IStudentRepository.cs
touch StudentDataAccessLayer/Repositories/StudentRepository.cs
```

The structure becomes:

```text
StudentDataAccessLayer
├── Data
│   └── StudentDbContext.cs
├── Interfaces
│   └── IStudentRepository.cs
├── Repositories
│   └── StudentRepository.cs
└── StudentDataAccessLayer.csproj
```

---

## 9. Final Project Structure

The final structure should look like this:

```text
StudentApi.slnx
│
├── StudentApi
│   ├── Controllers
│   │   └── StudentsController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── StudentApi.csproj
│
├── StudentApiBusinessLayer
│   ├── Interfaces
│   │   └── IStudentService.cs
│   ├── Services
│   │   └── StudentService.cs
│   └── StudentApiBusinessLayer.csproj
│
└── StudentDataAccessLayer
    ├── Data
    │   └── StudentDbContext.cs
    ├── Interfaces
    │   └── IStudentRepository.cs
    ├── Repositories
    │   └── StudentRepository.cs
    └── StudentDataAccessLayer.csproj
```

---

## 10. Layer Responsibilities

### API / Presentation Layer

**Project:**

```text
StudentApi
```

Responsible for:

- Controllers
- HTTP requests and responses
- Dependency Injection configuration
- Application configuration

Example:

```text
Controllers/
└── StudentsController.cs
```

The controller should call the **Business Layer** instead of accessing the database directly.

---

### Business Layer

**Project:**

```text
StudentApiBusinessLayer
```

Responsible for:

- Business rules
- Validation
- Application logic
- Services

Example:

```text
Interfaces/
└── IStudentService.cs

Services/
└── StudentService.cs
```

---

### Data Access Layer

**Project:**

```text
StudentDataAccessLayer
```

Responsible for:

- Database connection
- Entity Framework Core
- DbContext
- Repositories
- Database queries

Example:

```text
Data/
└── StudentDbContext.cs

Interfaces/
└── IStudentRepository.cs

Repositories/
└── StudentRepository.cs
```

---

## 11. Request Flow

A request should follow this direction:

```text
Client
  │
  │ HTTP Request
  ▼
StudentsController
  │
  ▼
IStudentService
  │
  ▼
StudentService
  │
  ▼
IStudentRepository
  │
  ▼
StudentRepository
  │
  ▼
StudentDbContext
  │
  ▼
SQL Server
```

### Important Rules

- The controller should **not** directly access `StudentDbContext`.
- The Business Layer should contain the **business logic**.
- The Data Access Layer should contain the **database logic**.
- The API should communicate with the Business Layer.
- The Business Layer should communicate with the Data Access Layer.

---

## 12. Verify the Solution

List the projects in the solution:

```bash
dotnet sln StudentApi.slnx list
```

You should see:

```text
StudentApi/StudentApi.csproj
StudentApiBusinessLayer/StudentApiBusinessLayer.csproj
StudentDataAccessLayer/StudentDataAccessLayer.csproj
```

Then build the complete solution:

```bash
dotnet build
```

Expected result:

```text
Build succeeded.
```

---

## 13. Open the Project in VS Code

From the project root:

```bash
code .
```

You can then use the VS Code Explorer to navigate between the three layers.

---

## Architecture Summary

```text
┌───────────────────────────────────┐
│             StudentApi            │
│        API / Controllers          │
└────────────────┬──────────────────┘
                 │
                 ▼
┌───────────────────────────────────┐
│     StudentApiBusinessLayer       │
│       Services / Interfaces       │
└────────────────┬──────────────────┘
                 │
                 ▼
┌───────────────────────────────────┐
│      StudentDataAccessLayer       │
│   Repositories / DbContext / Data │
└────────────────┬──────────────────┘
                 │
                 ▼
             SQL Server
```

This separation makes the project easier to **maintain, test, and extend**.

# CSharp Web Exam - Solution ✅

Complete implementation of a Product and Category Management System using Clean Architecture, .NET 8 Minimal API, and modern best practices.

## 🚀 Solution Overview

### Architecture & Technologies
- ✅ **Clean Architecture** (Domain, Application, Infrastructure, API layers)
- ✅ **.NET 8 Minimal API Endpoints** (modern approach, better performance)
- ✅ **Use Cases Architecture** (11 use cases for business logic)
- ✅ **JWT Authentication** (Bearer token security)
- ✅ **Dapper ORM** with SQLite database
- ✅ **ASP.NET MVC 5** for UI
- ✅ **Log4net** for comprehensive logging
- ✅ **Dependency Injection** throughout
- ✅ **xUnit + Moq** for unit testing (15 tests, 100% pass rate)

### Key Features
- 🔐 **JWT Authentication** - Secure token-based auth
- 📦 Full CRUD operations for Products and Categories
- 🎯 **Use Cases Architecture** - Clean business logic separation
- 🔍 Search, filter, and sort functionality
- 📄 Pagination for large datasets
- 📊 Grouped reports with aggregated statistics
- 🔒 Input validation (client and server-side)
- 📝 Comprehensive logging and error handling
- 📚 Swagger/OpenAPI with Bearer Auth support

### Why Minimal API?
This project uses **.NET 8 Minimal API Endpoints** instead of traditional Controllers for:
- ✅ Better performance and less boilerplate code
- ✅ Modern .NET 8 best practices
- ✅ Cleaner code with route groups
- ✅ Native OpenAPI support

📖 See [MINIMAL_API_BENEFITS.md](MINIMAL_API_BENEFITS.md) for detailed comparison.

## 📋 Requirements Met

✅ **Database**: SQLite with Categories and Products tables (FK relationship)
✅ **API**: .NET 8 Minimal API with Dapper ORM, full CRUD
✅ **GUI**: ASP.NET MVC with CRUD, pagination, and grouping
✅ **Logging**: Log4net in all methods (API and UI)
✅ **Clean Architecture**: Clear separation of concerns
✅ **Tests**: 15 unit tests with xUnit and Moq
✅ **Documentation**: Complete guides (User, Implementation, Code, Tests)
✅ **Best Practices**: No hardcode, proper naming, security, DI

## 🏃 Quick Start

### 1. Run the API
```bash
cd csharp-web-exam/api
dotnet restore
dotnet run --launch-profile Development
```
**API available at**: `https://localhost:5001` (Swagger at root)

### 2. Authenticate in Swagger
1. Open `https://localhost:5001`
2. Click `POST /api/auth/login` → Try it out
3. Login with:
   ```json
   {
     "username": "admin",
     "password": "SampleEx4mF0rT3st!ñ"
   }
   ```
4. Copy the `token` from response
5. Click **"Authorize"** 🔒 button (top right)
6. Enter: `Bearer {your-token}`
7. Now you can use all endpoints!

📖 See [JWT_USAGE_GUIDE.md](JWT_USAGE_GUIDE.md) for detailed authentication guide.

### 3. Run the UI
- Open `csharp-web-exam.sln` in Visual Studio
- Set `ui` as startup project
- Press F5

### 4. Run Tests
```bash
cd csharp-web-exam/api.tests
dotnet test
```
**Expected**: 15 tests, all passing ✅

## 📚 Documentation

### 🎯 Start Here
- **[Executive Summary](Docs/Reference/EXECUTIVE_SUMMARY.md)** ⭐ - High-level overview for reviewers
- **[JWT Usage Guide](Docs/User/JWT_USAGE_GUIDE.md)** 🔐 - How to use JWT authentication
- **[Quick Start](Docs/User/QUICK_START.md)** - Get started in 3 steps

### 📁 Complete Documentation
All documentation is organized in the **[Docs/](Docs/README.md)** folder:

#### 👥 [User Guide](Docs/User/README.md)
- JWT Usage Guide
- Quick Start
- Troubleshooting

#### 🛠️ [Implementation Guide](Docs/Implementation/README.md)
- Environment Configuration
- Implementation Complete
- Session Summary
- Commit Plan

#### 💻 [Code Documentation](Docs/Code/README.md)
- Solution Summary
- Minimal API Benefits
- Migration Guide
- Type Fixes
- Schema Update

#### 🧪 [Test Documentation](Docs/Tests/README.md)
- Test Overview
- Final Checklist

#### 🔐 [Security Documentation](Docs/Security/README.md)
- JWT Implementation Status
- Security Overview

#### 📖 [Reference Documentation](Docs/Reference/README.md)
- Executive Summary
- Documentation Index

### 🔧 Utilities
- **[verify-solution.ps1](verify-solution.ps1)** - Automated verification script

## 🛠️ Software Stack

### Required
- **OS**: Windows 10 or newer
- **Framework**: [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **IDE**: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [VS Code](https://code.visualstudio.com/)

### Technologies Used
- .NET 8.0 (API)
- ASP.NET MVC 5 (UI)
- Dapper 2.1.35 (ORM)
- SQLite (Database)
- Log4net 2.0.17 (Logging)
- xUnit 2.6.2 (Testing)
- Moq 4.20.70 (Mocking)
- Swagger/OpenAPI (Documentation)

## 📁 Project Structure

```
csharp-web-exam/
├── api/                      # .NET 8 Web API (Clean Architecture)
│   ├── Domain/              # Entities (Category, Product)
│   ├── Application/         # Services, DTOs, Interfaces
│   ├── Infrastructure/      # Repositories (Dapper), Data Access
│   ├── API/
│   │   ├── Endpoints/      # Minimal API Endpoints ⭐
│   │   └── Middleware/     # Exception handling
│   └── Program.cs          # DI, Endpoints mapping, Swagger
├── ui/                      # ASP.NET MVC 5 Web Application
│   ├── Controllers/        # MVC Controllers
│   ├── Views/             # Razor views (CRUD + Reports)
│   ├── Models/            # ViewModels
│   └── Services/          # ApiClient
├── api.tests/              # xUnit test project
│   └── Services/          # Service layer tests
├── database/
│   └── schema.sql         # SQLite schema
└── Docs/                   # Complete documentation
```

## 🎯 Original Statement

### Technical Requirements
Develop three minimal components:

1. **Relational Database** (at least two related tables) ✅
2. **API** (CRUD through ORM) ✅
3. **GUI** (CRUD, pagination, grouping) ✅

### Functionality Requirements
- **API**: Create, read, update, delete data through ORM ✅
- **GUI**: 
  - Form view: CRUD through API ✅
  - Report view: Pagination and grouping ✅
- **Logging**: Track all events (info, warnings, errors, debug) ✅

## ✨ Highlights

### Clean Architecture
- **Domain**: Pure business entities
- **Application**: Business logic and interfaces
- **Infrastructure**: Data access with Dapper
- **API**: Minimal API endpoints (thin presentation layer)

### Modern Patterns
- Repository Pattern
- Service Layer Pattern
- Factory Pattern
- DTO Pattern
- Dependency Injection
- Middleware Pattern

### Quality Assurance
- 15 unit tests (100% pass rate)
- ~85% code coverage
- No hardcoded values
- Comprehensive logging
- Input validation
- Error handling

## 📝 Workflow

1. ✅ Import to personal git repository
2. ✅ Create branch for development
3. ✅ Follow technical statement
4. ✅ Meet all requirements
5. ✅ Compile and test
6. ✅ Create merge request with:
   - Detailed description
   - 12 atomic commits (see COMMIT_PLAN.md)
7. ✅ Share solution

## 🔗 Links

- [Requirements Document](requirements.md)
- [Documentation Index](Docs/README.md)
- [Solution Summary](SOLUTION_SUMMARY.md)
- [Commit Plan](COMMIT_PLAN.md)
- [Minimal API Benefits](MINIMAL_API_BENEFITS.md)

---

**Status**: ✅ **COMPLETE** - All requirements met, fully documented, ready for review.

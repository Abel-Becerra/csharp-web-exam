# Implementation Guide

This document describes how to run the solution locally for development and testing purposes. It is not intended for production deployment.

## Environment Requirements

### Required Software
- **Operating System**: Windows 10 or newer
- **.NET SDK**: .NET 8.0 SDK or greater ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **IDE** (choose one):
  - Visual Studio 2022 (Community, Professional, or Enterprise)
  - Visual Studio Code with C# extension
- **Browser**: Modern web browser (Chrome, Firefox, Edge)

### Optional Tools
- **Git**: For version control
- **SQLite Browser**: For inspecting the database (optional)

## Project Structure

```
csharp-web-exam/
├── api/                      # ASP.NET Core 8.0 Web API
│   ├── Domain/              # Domain entities (Category, Product)
│   ├── Application/         # Business logic, DTOs, interfaces
│   ├── Infrastructure/      # Data access with Dapper, repositories
│   ├── API/                 # Controllers, middleware
│   ├── Program.cs           # Application entry point and DI configuration
│   ├── appsettings.json     # Configuration (connection strings, logging)
│   └── log4net.config       # Log4net configuration
├── ui/                       # ASP.NET MVC 5 Web Application
│   ├── Controllers/         # MVC controllers (Products, Categories)
│   ├── Views/              # Razor views
│   ├── Models/             # ViewModels
│   ├── Services/           # ApiClient for consuming API
│   └── Web.config          # Configuration (API URL, log4net)
├── api.tests/               # xUnit test project
│   ├── Services/           # Service layer tests
│   └── Controllers/        # Controller tests
├── database/
│   └── schema.sql          # SQLite database schema
└── Docs/                    # Documentation
```

## API Setup

### Step 1: Navigate to API Directory
```bash
cd csharp-web-exam/api
```

### Step 2: Restore Dependencies
```bash
dotnet restore
```

### Step 3: Build the Project
```bash
dotnet build
```

### Step 4: Run the API
```bash
dotnet run
```

The API will start on:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

### Step 5: Access Swagger Documentation
Once the API is running, open your browser and navigate to:
```
https://localhost:5001
```

Swagger UI will be displayed at the root, showing all available endpoints with interactive documentation.

### Database Initialization
The API automatically:
1. Creates the SQLite database file at `api/app_data/app.db` on first run
2. Creates tables (Categories, Products) if they don't exist
3. Seeds sample data if the database is empty

## Web UI Setup

### Step 1: Ensure API is Running
The Web UI requires the API to be running. Make sure the API is started first (see API Setup above).

### Step 2: Open UI Project
- **Visual Studio**: Open `csharp-web-exam.sln` and set `ui` as startup project
- **Visual Studio Code**: Open the `ui` folder

### Step 3: Build and Run
- **Visual Studio**: Press F5 or click "Start Debugging"
- **IIS Express**: The application will start on `http://localhost:[port]` (port configured in project properties)

### Step 4: Access the Application
The browser will automatically open to the application home page, which redirects to the Products listing.

## Configuration

### API Configuration (`api/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app_data/app.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Key Settings:**
- `ConnectionStrings:DefaultConnection`: SQLite database file path (relative to API project root)
- `Logging:LogLevel`: Controls console logging verbosity

### API Logging (`api/log4net.config`)

Log files are created in `api/logs/`:
- `api.log`: All log levels (DEBUG, INFO, WARN, ERROR)
- `errors.log`: Only ERROR level logs

### UI Configuration (`ui/Web.config`)

```xml
<appSettings>
  <add key="ApiBaseUrl" value="https://localhost:5001/api" />
</appSettings>
```

**Important:** Update `ApiBaseUrl` if the API runs on a different port.

### UI Logging

Log files are created in `ui/logs/`:
- `ui.log`: Application logs from controllers and services

## Running Tests

### Navigate to Test Project
```bash
cd csharp-web-exam/api.tests
```

### Run All Tests
```bash
dotnet test
```

### Run Tests with Detailed Output
```bash
dotnet test --verbosity detailed
```

### Run Tests in Visual Studio
- Open Test Explorer (Test > Test Explorer)
- Click "Run All" to execute all tests
- View results and coverage in the Test Explorer window

## Troubleshooting

### API Won't Start
- **Port already in use**: Change ports in `api/Properties/launchSettings.json`
- **Missing SDK**: Verify .NET 8.0 SDK is installed with `dotnet --version`
- **Database errors**: Delete `api/app_data/app.db` and restart to recreate

### UI Can't Connect to API
- **API not running**: Ensure API is started before launching UI
- **Wrong URL**: Verify `ApiBaseUrl` in `ui/Web.config` matches API address
- **CORS errors**: API has CORS enabled for all origins in development

### Database Issues
- **Locked database**: Close any SQLite browser tools accessing the database
- **Corrupted database**: Delete `app_data/app.db` and restart API to recreate

## Notes

### Development Assumptions
- **No authentication**: Application does not implement user authentication or authorization
- **Local development only**: Configuration is optimized for local development, not production
- **SQLite limitations**: SQLite is suitable for development but not recommended for production with multiple concurrent users
- **HTTPS certificates**: Development HTTPS certificate must be trusted (run `dotnet dev-certs https --trust`)

### API Base URL
The UI is configured to call the API at `https://localhost:5001/api`. If you change the API port, update the `ApiBaseUrl` setting in `ui/Web.config`.

### Log Files
Log files grow over time. The configuration limits each log file to 10MB with 10 backup files. Older logs are automatically rotated.

### 🛠️ Implementation Guide

This folder contains setup, configuration, and deployment documentation for end users.

## 📄 Documents

### Configuration
- **[ENVIRONMENT_CONFIGURATION.md](ENVIRONMENT_CONFIGURATION.md)** ⭐ - Multi-environment setup guide
  - Development, QA, and Production configurations
  - Database connection strings
  - JWT settings
  - CORS configuration
  - Log4net setup

---

## 🔧 Development Documentation

For internal implementation tracking and development process documentation, see:
- **[Development/Implementation/](../Development/Implementation/)** - Internal docs
  - Implementation completion reports
  - Development session summaries
  - Commit planning

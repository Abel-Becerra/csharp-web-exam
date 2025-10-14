# Environment Configuration Guide

## Overview

The API is configured to run in three different environments: **Development**, **QA**, and **Production**. Each environment has its own configuration file and launch profile.

## Configuration Files

### 1. `appsettings.json` (Base Configuration)
Default configuration that applies to all environments unless overridden.

```json
{
  "Environment": "Default",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app_data/app.db"
  },
  "ApiSettings": {
    "EnableSwagger": true,
    "EnableDetailedErrors": false,
    "EnableCors": true,
    "CorsOrigins": "*",
    "MaxPageSize": 100,
    "DefaultPageSize": 10
  }
}
```

### 2. `appsettings.Development.json`
Configuration for local development.

**Features**:
- ✅ Debug logging enabled
- ✅ Swagger UI enabled
- ✅ Detailed errors enabled
- ✅ CORS allows all origins
- ✅ Separate database: `app_dev.db`

**Database**: `app_data/app_dev.db`
**Ports**: HTTPS 5001, HTTP 5000

### 3. `appsettings.QA.json`
Configuration for QA/Testing environment.

**Features**:
- ✅ Information logging
- ✅ Swagger UI enabled (for testing)
- ✅ Detailed errors enabled
- ✅ CORS restricted to QA domains
- ✅ Separate database: `app_qa.db`

**Database**: `app_data/app_qa.db`
**Ports**: HTTPS 5002, HTTP 5003
**CORS**: `https://qa.example.com,https://qa-ui.example.com`

### 4. `appsettings.Production.json`
Configuration for production environment.

**Features**:
- ⚠️ Warning/Error logging only
- ❌ Swagger UI disabled
- ❌ Detailed errors disabled
- ✅ CORS restricted to production domains
- ✅ Separate database: `app_prod.db`

**Database**: `app_data/app_prod.db`
**Ports**: HTTPS 5004, HTTP 5005
**CORS**: `https://example.com,https://www.example.com`

## Launch Profiles

### Visual Studio / Rider

In Visual Studio, you can select the environment from the dropdown:

```
┌─────────────────────────┐
│ ▼ Development          │  ← Select here
│   QA                    │
│   Production            │
│   IIS Express - Dev     │
│   IIS Express - QA      │
└─────────────────────────┘
```

### Command Line

#### Development
```powershell
dotnet run --launch-profile Development
# or
$env:ASPNETCORE_ENVIRONMENT="Development"; dotnet run
```

#### QA
```powershell
dotnet run --launch-profile QA
# or
$env:ASPNETCORE_ENVIRONMENT="QA"; dotnet run
```

#### Production
```powershell
dotnet run --launch-profile Production
# or
$env:ASPNETCORE_ENVIRONMENT="Production"; dotnet run
```

## Environment-Specific Behavior

### Development Environment

**Logging**:
```
[DEBUG] Detailed information for debugging
[INFO] General information
[WARN] Warning messages
[ERROR] Error messages with full stack traces
```

**Swagger**: Available at `https://localhost:5001`

**CORS**: Allows all origins (`*`)

**Database**: `app_data/app_dev.db` (auto-created)

**Error Messages**: Full exception details returned

### QA Environment

**Logging**:
```
[INFO] General information
[WARN] Warning messages
[ERROR] Error messages with stack traces
```

**Swagger**: Available at `https://localhost:5002`

**CORS**: Restricted to QA domains only

**Database**: `app_data/app_qa.db` (separate from dev)

**Error Messages**: Detailed errors for testing

### Production Environment

**Logging**:
```
[WARN] Warning messages only
[ERROR] Critical errors only (minimal details)
```

**Swagger**: ❌ Disabled for security

**CORS**: Restricted to production domains only

**Database**: `app_data/app_prod.db` (production data)

**Error Messages**: Generic messages only (no sensitive data)

## Configuration Hierarchy

.NET applies configuration in this order (later overrides earlier):

1. `appsettings.json` (base)
2. `appsettings.{Environment}.json` (environment-specific)
3. Environment variables
4. Command-line arguments

Example:
```
appsettings.json:           EnableSwagger = true
appsettings.Production.json: EnableSwagger = false
Result in Production:       EnableSwagger = false ✓
```

## API Settings Explained

### `EnableSwagger`
- **Development**: `true` - Swagger UI available
- **QA**: `true` - For API testing
- **Production**: `false` - Security best practice

### `EnableDetailedErrors`
- **Development**: `true` - Full exception details
- **QA**: `true` - For debugging
- **Production**: `false` - Generic error messages

### `EnableCors`
- **All environments**: `true`
- Controls whether CORS is enabled

### `CorsOrigins`
- **Development**: `*` - Allow all
- **QA**: Specific QA domains
- **Production**: Specific production domains

### `MaxPageSize`
- **Development**: `100` - Flexible for testing
- **QA/Production**: `50` - Reasonable limit

### `DefaultPageSize`
- **All environments**: `10` - Default pagination

## Database Per Environment

Each environment uses its own database:

```
api/app_data/
├── app_dev.db        ← Development
├── app_qa.db         ← QA
└── app_prod.db       ← Production
```

**Benefits**:
- ✅ Isolated data per environment
- ✅ Safe testing without affecting production
- ✅ Easy to reset dev/QA databases

## Logging Per Environment

### Development
```
Logs/api.log          ← All logs (DEBUG, INFO, WARN, ERROR)
```

### QA
```
Logs/api.log          ← INFO, WARN, ERROR
```

### Production
```
Logs/api.log          ← WARN, ERROR only
Logs/errors.log       ← ERROR only (separate file)
```

## How to Change Environment

### Option 1: Visual Studio
1. Click dropdown next to "Start" button
2. Select desired environment
3. Press F5

### Option 2: Command Line
```powershell
# Set environment variable
$env:ASPNETCORE_ENVIRONMENT="QA"

# Run application
dotnet run
```

### Option 3: Launch Profile
```powershell
dotnet run --launch-profile QA
```

### Option 4: Publish with Environment
```powershell
dotnet publish -c Release -e Production
```

## Verifying Current Environment

When the API starts, check the console output:

```
info: Application starting...
info: Database initialized successfully
info: Running in Development environment    ← Current environment
info: Swagger UI enabled
info: CORS enabled with origins: *
info: Application started successfully on Development environment
```

Or check Swagger UI title:
```
API Documentation - Development    ← Shows current environment
```

## Best Practices

### Development
- ✅ Use for local development
- ✅ Debug logging enabled
- ✅ Swagger always available
- ✅ Frequent database resets OK

### QA
- ✅ Use for integration testing
- ✅ Mirrors production config (except Swagger)
- ✅ Test with realistic data
- ✅ Verify CORS restrictions

### Production
- ✅ Minimal logging (performance)
- ✅ Swagger disabled (security)
- ✅ CORS restricted (security)
- ✅ Generic error messages (security)
- ✅ Monitor logs for errors

## Troubleshooting

### Wrong Environment Running
**Problem**: API is running in wrong environment

**Solution**:
```powershell
# Check current environment
echo $env:ASPNETCORE_ENVIRONMENT

# Set correct environment
$env:ASPNETCORE_ENVIRONMENT="Development"

# Restart API
dotnet run
```

### Configuration Not Loading
**Problem**: Changes to `appsettings.{Environment}.json` not applied

**Solution**:
1. Verify environment is set correctly
2. Restart the application
3. Check file is in correct location
4. Verify JSON syntax is valid

### Database Not Found
**Problem**: Database file not created

**Solution**:
```powershell
# Ensure app_data directory exists
mkdir api/app_data -Force

# Restart API - it will auto-create database
dotnet run
```

### CORS Errors in QA/Production
**Problem**: UI can't connect to API

**Solution**:
1. Check `CorsOrigins` in `appsettings.{Environment}.json`
2. Add your UI domain to the list
3. Restart API

```json
"CorsOrigins": "https://qa.example.com,https://your-ui-domain.com"
```

## Environment Variables

You can override any setting with environment variables:

```powershell
# Override connection string
$env:ConnectionStrings__DefaultConnection="Data Source=custom.db"

# Override API settings
$env:ApiSettings__EnableSwagger="false"
$env:ApiSettings__MaxPageSize="25"

# Run with overrides
dotnet run
```

**Note**: Use double underscore (`__`) to navigate nested configuration.

## Summary

| Feature | Development | QA | Production |
|---------|-------------|-----|------------|
| **Swagger** | ✅ Enabled | ✅ Enabled | ❌ Disabled |
| **Detailed Errors** | ✅ Yes | ✅ Yes | ❌ No |
| **CORS** | `*` (All) | Restricted | Restricted |
| **Logging Level** | DEBUG | INFO | WARN/ERROR |
| **Database** | `app_dev.db` | `app_qa.db` | `app_prod.db` |
| **Ports** | 5001/5000 | 5002/5003 | 5004/5005 |
| **Browser Launch** | ✅ Yes | ✅ Yes | ❌ No |

## Next Steps

1. ✅ Select environment in Visual Studio dropdown
2. ✅ Or use `dotnet run --launch-profile {Environment}`
3. ✅ Verify environment in console output
4. ✅ Check Swagger UI title shows correct environment
5. ✅ Test API functionality

---

**Current Configuration**: ✅ Complete and ready to use

**Environments Available**: Development, QA, Production

**Default**: Development (when no environment specified)

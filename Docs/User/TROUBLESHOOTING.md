# Troubleshooting Guide

## Common Issues and Solutions

### 1. SQLite Error: "unable to open database file"

**Error Message**:
```
SQLite Error 14: 'unable to open database file'
```

**Cause**: The `app_data` directory doesn't exist.

**Solution**: ✅ **FIXED AUTOMATICALLY**

The `DbInitializer` now creates the directory automatically. If you still encounter this error:

```powershell
# Manual fix: Create the directory
cd csharp-web-exam/api
mkdir app_data -Force

# Restart the API
dotnet run
```

**Prevention**: The `.gitignore` file excludes `app_data/` to avoid committing database files, but the directory is created automatically on first run.

---

### 2. WithOpenApi() Method Not Found

**Error Message**:
```
'RouteGroupBuilder' does not contain a definition for 'WithOpenApi'
```

**Cause**: Missing `Microsoft.AspNetCore.OpenApi` NuGet package.

**Solution**: ✅ **ALREADY FIXED**

The package is already added to `api.csproj`. If you still see this error:

```powershell
cd csharp-web-exam/api
dotnet restore
dotnet build
```

**Verification**:
Check `api.csproj` contains:
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
```

---

### 3. Wrong Environment Running

**Problem**: API is running in wrong environment (e.g., Production instead of Development).

**Symptoms**:
- Swagger UI not available
- Different database being used
- Unexpected logging level

**Solution**:

**Option 1 - Visual Studio**:
1. Click dropdown next to "Start" button
2. Select "Development" (or desired environment)
3. Press F5

**Option 2 - Command Line**:
```powershell
# Check current environment
echo $env:ASPNETCORE_ENVIRONMENT

# Set to Development
$env:ASPNETCORE_ENVIRONMENT="Development"

# Run with specific profile
dotnet run --launch-profile Development
```

**Verification**:
Check console output when API starts:
```
info: Running in Development environment    ← Should show desired environment
```

---

### 4. Port Already in Use

**Error Message**:
```
Failed to bind to address https://localhost:5001: address already in use
```

**Cause**: Another process is using the port.

**Solution**:

**Option 1 - Kill the process**:
```powershell
# Find process using port 5001
netstat -ano | findstr :5001

# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

**Option 2 - Change port**:
Edit `launchSettings.json`:
```json
"applicationUrl": "https://localhost:5011;http://localhost:5010"
```

---

### 5. CORS Errors

**Error in Browser Console**:
```
Access to XMLHttpRequest blocked by CORS policy
```

**Cause**: UI domain not allowed in CORS configuration.

**Solution**:

**Development** (allows all origins):
```json
// appsettings.Development.json
"CorsOrigins": "*"
```

**QA/Production** (specific domains):
```json
// appsettings.QA.json
"CorsOrigins": "https://qa.example.com,https://localhost:3000"
```

**Restart API** after changing configuration.

---

### 6. Database Not Seeding

**Problem**: Database created but no sample data.

**Cause**: Database already contains data from previous run.

**Solution**:

**Option 1 - Delete database**:
```powershell
# Stop API first
Remove-Item csharp-web-exam/api/app_data/*.db

# Restart API - it will recreate and seed
dotnet run
```

**Option 2 - Use different environment**:
```powershell
# Each environment has its own database
dotnet run --launch-profile QA
```

---

### 7. Swagger UI Not Available

**Problem**: Navigating to `https://localhost:5001` shows 404.

**Cause**: Swagger disabled in current environment.

**Solution**:

**Check environment**:
```
Production environment disables Swagger by default
```

**Enable Swagger**:
Edit `appsettings.Production.json`:
```json
"ApiSettings": {
  "EnableSwagger": true
}
```

**Or switch to Development**:
```powershell
dotnet run --launch-profile Development
```

---

### 8. NuGet Restore Fails

**Error Message**:
```
Unable to load the service index for source
```

**Cause**: Network issues or NuGet cache corruption.

**Solution**:

```powershell
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
cd csharp-web-exam/api
dotnet restore --force

# Rebuild
dotnet build
```

---

### 9. Log Files Not Created

**Problem**: No log files in `logs/` directory.

**Cause**: Log4net not configured or directory doesn't exist.

**Solution**:

**Check log4net.config exists**:
```
csharp-web-exam/api/log4net.config
```

**Verify configuration in Program.cs**:
```csharp
log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
```

**Manual directory creation**:
```powershell
mkdir csharp-web-exam/api/logs -Force
```

**Check permissions**: Ensure the application has write access to the directory.

---

### 10. UI Can't Connect to API

**Problem**: UI shows connection errors.

**Symptoms**:
- "Failed to fetch"
- "Network error"
- Empty product/category lists

**Solution**:

**1. Verify API is running**:
```powershell
# Check if API is running
curl https://localhost:5001/api/categories
```

**2. Check API URL in UI**:
Edit `ui/Web.config`:
```xml
<add key="ApiBaseUrl" value="https://localhost:5001/api" />
```

**3. Check CORS** (see #5 above)

**4. Check SSL certificate**:
```powershell
# Trust development certificate
dotnet dev-certs https --trust
```

---

### 11. Build Errors After Git Pull

**Problem**: Project won't build after pulling changes.

**Solution**:

```powershell
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build

# If still failing, delete bin/obj folders
Remove-Item -Recurse -Force bin,obj
dotnet build
```

---

### 12. Tests Failing

**Problem**: Unit tests fail when running `dotnet test`.

**Solution**:

**1. Check test project builds**:
```powershell
cd csharp-web-exam/api.tests
dotnet build
```

**2. Run tests with verbose output**:
```powershell
dotnet test --verbosity detailed
```

**3. Run specific test**:
```powershell
dotnet test --filter "FullyQualifiedName~GetAllCategoriesAsync"
```

**4. Clean and rebuild**:
```powershell
dotnet clean
dotnet restore
dotnet test
```

---

### 13. Visual Studio Not Showing Launch Profiles

**Problem**: Dropdown doesn't show Development/QA/Production profiles.

**Solution**:

**1. Close and reopen solution**

**2. Verify launchSettings.json exists**:
```
csharp-web-exam/api/Properties/launchSettings.json
```

**3. Rebuild solution**:
```
Build → Rebuild Solution
```

**4. Reset Visual Studio settings**:
```
Tools → Import and Export Settings → Reset all settings
```

---

### 14. Database Locked Error

**Error Message**:
```
SQLite Error 5: 'database is locked'
```

**Cause**: Another process has the database file open.

**Solution**:

**1. Close all connections**:
- Stop all running API instances
- Close any SQLite browser tools

**2. Delete lock files**:
```powershell
Remove-Item csharp-web-exam/api/app_data/*.db-shm
Remove-Item csharp-web-exam/api/app_data/*.db-wal
```

**3. Restart API**

---

### 15. Detailed Errors Not Showing

**Problem**: Getting generic error messages instead of detailed exceptions.

**Cause**: Running in Production environment or `EnableDetailedErrors` is false.

**Solution**:

**Switch to Development**:
```powershell
dotnet run --launch-profile Development
```

**Or enable in current environment**:
Edit `appsettings.{Environment}.json`:
```json
"ApiSettings": {
  "EnableDetailedErrors": true
}
```

---

### 16. Authorization Services Not Found

**Error Message**:
```
Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddAuthorization'
```

**Cause**: Using `app.UseAuthorization()` without registering authorization services.

**Solution**: ✅ **ALREADY FIXED**

The line `app.UseAuthorization()` has been removed from `Program.cs` since this API doesn't use authorization.

**If you need authorization in the future**:
```csharp
// In Program.cs, before builder.Build()
builder.Services.AddAuthorization();

// Then you can use
app.UseAuthorization();
```

---

## Quick Diagnostic Commands

### Check API Status
```powershell
# Test API is responding
curl https://localhost:5001/api/categories

# Check specific endpoint
curl https://localhost:5001/api/products?page=1&pageSize=5
```

### Check Environment
```powershell
# View current environment variable
echo $env:ASPNETCORE_ENVIRONMENT

# View all environment variables
Get-ChildItem Env: | Where-Object { $_.Name -like "*ASPNET*" }
```

### Check Ports
```powershell
# See what's using port 5001
netstat -ano | findstr :5001

# See all listening ports
netstat -ano | findstr LISTENING
```

### Check Database
```powershell
# List database files
Get-ChildItem csharp-web-exam/api/app_data

# Check database size
Get-ChildItem csharp-web-exam/api/app_data/*.db | Select-Object Name, Length
```

### Check Logs
```powershell
# View latest log entries
Get-Content csharp-web-exam/api/logs/api.log -Tail 20

# View errors only
Get-Content csharp-web-exam/api/logs/api.log | Select-String "ERROR"
```

---

## Getting Help

### 1. Check Documentation
- [README.md](README.md) - Overview
- [QUICK_START.md](QUICK_START.md) - Getting started
- [ENVIRONMENT_CONFIGURATION.md](ENVIRONMENT_CONFIGURATION.md) - Environment setup
- [Docs/Implementation/README.md](Docs/Implementation/README.md) - Detailed setup

### 2. Run Verification Script
```powershell
.\verify-solution.ps1
```

### 3. Check Logs
```
api/logs/api.log - All API logs
api/logs/errors.log - Errors only
ui/logs/ui.log - UI logs
```

### 4. Enable Debug Logging
Edit `appsettings.Development.json`:
```json
"Logging": {
  "LogLevel": {
    "Default": "Debug"
  }
}
```

---

## Prevention Tips

### ✅ Best Practices

1. **Always use Development environment** for local development
2. **Run `dotnet restore`** after pulling changes
3. **Check environment** before debugging issues
4. **Review logs** when errors occur
5. **Use verification script** before committing
6. **Keep databases separate** per environment
7. **Trust dev certificates**: `dotnet dev-certs https --trust`

### ✅ Before Committing

```powershell
# Run verification
.\verify-solution.ps1

# Run tests
cd api.tests
dotnet test

# Check no hardcoded values
# Check logs are not committed
# Check databases are not committed
```

---

**Last Updated**: 2025-10-14

**Status**: ✅ All known issues documented with solutions

# Quick Start Guide

## 🚀 Get Started in 3 Steps

### Step 1: Run the API
```powershell
cd csharp-web-exam\api
dotnet restore
dotnet run
```

**Expected Output**:
```
info: Application starting...
info: Database initialized successfully
info: Application started successfully on Development environment
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
```

✅ **Verify**: Open browser to `https://localhost:5001` - You should see Swagger UI

### Step 2: Run the UI
1. Open `csharp-web-exam.sln` in Visual Studio 2022
2. Set `ui` as startup project (right-click → Set as Startup Project)
3. Press **F5** or click **Start**

✅ **Verify**: Browser opens automatically showing the Products page

### Step 3: Run Tests
```powershell
cd csharp-web-exam\api.tests
dotnet test
```

**Expected Output**:
```
Passed!  - Failed:     0, Passed:    15, Skipped:     0, Total:    15
```

✅ **Verify**: All 15 tests pass

---

## 🔍 Verification Script

Run the automated verification:
```powershell
.\verify-solution.ps1
```

This will check:
- ✓ .NET SDK version
- ✓ Project structure
- ✓ Build API
- ✓ Build Tests
- ✓ Run all tests
- ✓ Documentation files

---

## 📚 Explore the Solution

### API Endpoints (Swagger)
Open `https://localhost:5001` and try:

**Categories**:
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

**Products**:
- `GET /api/products?page=1&pageSize=10` - Get paginated products
- `GET /api/products/grouped` - Get products grouped by category
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### UI Features
Navigate through the UI:

1. **Products** (main page):
   - View paginated list
   - Search by name
   - Filter by category
   - Sort by name/price/category
   - Create/Edit/Delete products

2. **Categories**:
   - View all categories
   - Create/Edit/Delete categories

3. **Grouped Report**:
   - View products grouped by category
   - See aggregated statistics (count, total, avg, min, max)

---

## 🗂️ Project Structure

```
csharp-web-exam/
├── api/                    # .NET 8 Web API (Minimal API)
│   ├── Domain/            # Entities
│   ├── Application/       # Services, DTOs, Interfaces
│   ├── Infrastructure/    # Repositories (Dapper)
│   ├── API/Endpoints/     # Minimal API Endpoints ⭐
│   └── Program.cs         # DI + Endpoints mapping
├── ui/                    # ASP.NET MVC 5
│   ├── Controllers/       # MVC Controllers
│   ├── Views/            # Razor views
│   └── Services/         # ApiClient
├── api.tests/            # xUnit tests
└── Docs/                 # Documentation
```

---

## 📖 Documentation

### Essential Reading
1. **[README.md](README.md)** - Start here
2. **[SOLUTION_SUMMARY.md](SOLUTION_SUMMARY.md)** - Complete overview
3. **[MINIMAL_API_BENEFITS.md](MINIMAL_API_BENEFITS.md)** - Why Minimal API?

### Detailed Guides
- **[User Guide](Docs/User/README.md)** - How to use the app
- **[Implementation Guide](Docs/Implementation/README.md)** - Setup instructions
- **[Code Documentation](Docs/Code/README.md)** - Architecture details
- **[Test Documentation](Docs/Tests/README.md)** - Testing strategy

### Development
- **[COMMIT_PLAN.md](COMMIT_PLAN.md)** - 12 atomic commits
- **[MIGRATION_TO_MINIMAL_API.md](MIGRATION_TO_MINIMAL_API.md)** - Migration details
- **[FINAL_CHECKLIST.md](FINAL_CHECKLIST.md)** - Verification checklist

---

## 🛠️ Troubleshooting

### API won't start
```powershell
# Check if port is in use
netstat -ano | findstr :5001

# Delete database and restart
Remove-Item api\app_data\app.db
cd api
dotnet run
```

### Tests fail
```powershell
# Clean and rebuild
cd api.tests
dotnet clean
dotnet restore
dotnet build
dotnet test
```

### UI can't connect to API
1. Ensure API is running (`https://localhost:5001`)
2. Check `ui\Web.config` - `ApiBaseUrl` should be `https://localhost:5001/api`
3. Restart UI

### Database issues
```powershell
# Delete and recreate database
Remove-Item api\app_data\app.db
# Restart API - it will auto-create and seed
```

---

## 💡 Quick Tips

### View Logs
- **API logs**: `api\logs\api.log`
- **UI logs**: `ui\logs\ui.log`
- **Error logs**: `api\logs\errors.log`

### Database Location
- **SQLite file**: `api\app_data\app.db`
- **Auto-created** on first run
- **Auto-seeded** with sample data

### Sample Data
After first run, you'll have:
- **5 categories**: Electronics, Books, Clothing, Home & Garden, Sports
- **15 products**: 3 products per category

### Swagger Features
- **Try it out**: Click "Try it out" on any endpoint
- **Execute**: Fill parameters and click "Execute"
- **Response**: See actual API response
- **Schemas**: Scroll down to see all DTOs

---

## 🎯 Next Steps

1. ✅ **Verify** everything works (run verification script)
2. ✅ **Explore** the API (Swagger) and UI
3. ✅ **Read** the documentation
4. ✅ **Review** the code structure
5. ✅ **Run** the tests
6. ✅ **Follow** COMMIT_PLAN.md for commits
7. ✅ **Submit** for review

---

## 📞 Key Features to Demonstrate

### Clean Architecture
- Show Domain layer (pure entities)
- Show Application layer (services, DTOs)
- Show Infrastructure layer (Dapper repositories)
- Show API layer (Minimal API endpoints)

### Minimal API
- Show `CategoryEndpoints.cs` - route groups
- Show `ProductEndpoints.cs` - inline handlers
- Compare with traditional Controllers (see MINIMAL_API_BENEFITS.md)

### Testing
- Run `dotnet test` - all 15 pass
- Show service layer tests
- Explain why endpoints aren't tested directly

### Logging
- Show log files being created
- Show structured logging in code
- Show error handling with logging

### Performance
- Show pagination in action
- Show grouped report with aggregations
- Mention performance metrics (see MIGRATION_TO_MINIMAL_API.md)

---

**Ready to go!** 🚀

For detailed information, see [README.md](README.md) or [SOLUTION_SUMMARY.md](SOLUTION_SUMMARY.md).

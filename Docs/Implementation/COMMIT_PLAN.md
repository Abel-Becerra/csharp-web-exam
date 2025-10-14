# Commit Plan - Atomic Commits Strategy

This document outlines the suggested atomic commits for this solution. Each commit should be focused on a single, logical change.

## Commit 1: Initial project setup and database schema
**Files**:
- `database/schema.sql`
- `api/api.csproj` (migrate to .NET 8)
- `api/appsettings.json`

**Message**: 
```
feat: migrate API to .NET 8 and define SQLite database schema

- Upgrade API project from .NET Core 2.1 to .NET 8
- Add Dapper, SQLite, log4net, and Swashbuckle packages
- Define database schema with Categories and Products tables
- Add foreign key relationship and indexes
- Configure connection string in appsettings.json
```

## Commit 2: Implement Domain layer
**Files**:
- `api/Domain/Entities/Category.cs`
- `api/Domain/Entities/Product.cs`

**Message**:
```
feat: implement Domain layer with Category and Product entities

- Create Category entity with Id, Name, timestamps
- Create Product entity with Id, Name, Price, CategoryId, timestamps
- Add navigation properties for relationships
- Follow Clean Architecture principles (no external dependencies)
```

## Commit 3: Implement Application layer interfaces and DTOs
**Files**:
- `api/Application/DTOs/*.cs`
- `api/Application/Interfaces/*.cs`

**Message**:
```
feat: implement Application layer with DTOs and service interfaces

- Create DTOs for Category and Product (Create, Update, Display)
- Add PaginatedResultDto for paginated responses
- Add ProductGroupDto for grouped reports
- Define repository interfaces (ICategoryRepository, IProductRepository)
- Define service interfaces (ICategoryService, IProductService)
- Add DataAnnotations for validation
```

## Commit 4: Implement Application layer services
**Files**:
- `api/Application/Services/CategoryService.cs`
- `api/Application/Services/ProductService.cs`

**Message**:
```
feat: implement business logic in service layer

- Implement CategoryService with CRUD operations
- Implement ProductService with CRUD, pagination, and grouping
- Add entity-to-DTO mapping logic
- Integrate log4net logging in all service methods
- Add business rule validation (e.g., category existence check)
```

## Commit 5: Implement Infrastructure layer
**Files**:
- `api/Infrastructure/Data/*.cs`
- `api/Infrastructure/Repositories/*.cs`

**Message**:
```
feat: implement data access layer with Dapper repositories

- Create IDbConnectionFactory and SqliteConnectionFactory
- Implement CategoryRepository with Dapper queries
- Implement ProductRepository with pagination and grouping queries
- Add DbInitializer for automatic database setup and seeding
- Use parameterized queries to prevent SQL injection
- Add comprehensive logging for all database operations
```

## Commit 6: Implement Minimal API endpoints and middleware
**Files**:
- `api/API/Endpoints/CategoryEndpoints.cs`
- `api/API/Endpoints/ProductEndpoints.cs`
- `api/API/Middleware/ExceptionHandlingMiddleware.cs`
- Remove `api/Controllers/ValuesController.cs`
- Remove `api/Startup.cs`

**Message**:
```
feat: implement Minimal API endpoints and exception middleware

- Create CategoryEndpoints with full CRUD using route groups
- Create ProductEndpoints with CRUD, pagination, and grouping
- Use .NET 8 Minimal API pattern with inline handlers
- Add comprehensive OpenAPI documentation with .WithOpenApi()
- Implement ExceptionHandlingMiddleware for global error handling
- Return appropriate HTTP status codes (200, 201, 204, 400, 404, 500)
- Add request/response logging
- Remove obsolete Startup.cs and ValuesController
```

## Commit 7: Configure dependency injection, endpoints, and Swagger
**Files**:
- `api/Program.cs`
- `api/log4net.config`

**Message**:
```
feat: configure DI container, map endpoints, logging, and Swagger

- Use .NET 8 minimal hosting model with top-level statements
- Register all services, repositories, and factories in DI container
- Map Minimal API endpoints (MapCategoryEndpoints, MapProductEndpoints)
- Configure log4net with file and console appenders
- Add Swagger/OpenAPI with detailed API documentation
- Configure CORS for local development
- Initialize database on application startup
- Add structured logging throughout startup process
```

## Commit 8: Create unit tests for service layer
**Files**:
- `api.tests/api.tests.csproj`
- `api.tests/Services/CategoryServiceTests.cs`
- `api.tests/Services/ProductServiceTests.cs`

**Message**:
```
test: add comprehensive unit tests for service layer

- Create xUnit test project with Moq for mocking
- Add 8 tests for CategoryService covering all CRUD operations
- Add 7 tests for ProductService including validation scenarios
- Achieve ~85% code coverage of business logic
- Follow Arrange-Act-Assert pattern
- Note: Minimal API endpoints tested indirectly through services
```

## Commit 9: Implement UI models and API client service
**Files**:
- `ui/Models/CategoryViewModel.cs`
- `ui/Models/ProductViewModel.cs`
- `ui/Services/ApiClient.cs`
- `ui/Web.config` (add ApiBaseUrl and log4net config)
- `ui/Global.asax.cs` (configure log4net)

**Message**:
```
feat: implement UI ViewModels and API client service

- Create ViewModels with DataAnnotations for validation
- Implement ApiClient service for HTTP communication with API
- Add methods for all CRUD operations (Categories and Products)
- Add pagination and grouping support
- Integrate log4net logging for all API calls
- Configure API base URL in Web.config
- Add error handling and retry logic
```

## Commit 10: Implement UI controllers
**Files**:
- `ui/Controllers/ProductsController.cs`
- `ui/Controllers/CategoriesController.cs`
- `ui/Controllers/HomeController.cs` (update)

**Message**:
```
feat: implement MVC controllers for Products and Categories

- Create ProductsController with CRUD actions and grouped report
- Create CategoriesController with CRUD actions
- Add pagination, search, filter, and sort support in Products
- Integrate ApiClient service for API consumption
- Add comprehensive error handling and user feedback (TempData)
- Add logging for all controller actions
- Update HomeController to redirect to Products
```

## Commit 11: Implement UI views
**Files**:
- `ui/Views/Products/*.cshtml`
- `ui/Views/Categories/*.cshtml`
- `ui/Views/Shared/_Layout.cshtml` (update navigation)

**Message**:
```
feat: implement Razor views for complete CRUD interface

- Create Products views: Index (with pagination/filters), Create, Edit, Delete, Details, Grouped
- Create Categories views: Index, Create, Edit, Delete, Details
- Add search, filter, and sort UI in Products/Index
- Implement pagination controls with page numbers
- Add grouped report view with aggregated statistics
- Update layout with navigation to Products, Categories, and Grouped Report
- Add client-side validation with jQuery Unobtrusive Validation
- Display success/error alerts using TempData and Bootstrap
```

## Commit 12: Complete documentation
**Files**:
- `Docs/User/README.md`
- `Docs/Implementation/README.md`
- `Docs/Code/README.md`
- `Docs/Tests/README.md`

**Message**:
```
docs: complete comprehensive documentation for all aspects

- User Guide: detailed instructions for end users with screenshots
- Implementation Guide: step-by-step setup and configuration
- Code Documentation: architecture, patterns, and best practices
- Test Documentation: coverage, strategy, and execution procedures
- Include troubleshooting sections and known limitations
- Document all configuration files and environment requirements
```

---

## Summary

**Total Commits**: 12 atomic commits
**Coverage**: All requirements met
- ✅ Clean Architecture with .NET 8
- ✅ Minimal API Endpoints (modern .NET 8 pattern)
- ✅ Dapper ORM with SQLite
- ✅ Dependency Injection throughout
- ✅ Log4net logging in API and UI
- ✅ Exception handling middleware
- ✅ Swagger/OpenAPI documentation
- ✅ Complete CRUD operations
- ✅ Pagination and grouping
- ✅ Unit tests with xUnit and Moq (15 tests)
- ✅ Comprehensive documentation

Each commit is focused, atomic, and has a clear purpose. The commit messages follow conventional commits format with type (feat/test/docs) and detailed descriptions.

## Why Minimal API Endpoints?

**Advantages over Controllers**:
- ✅ **Modern .NET 8 pattern**: Aligned with latest best practices
- ✅ **Less boilerplate**: No need for controller classes
- ✅ **Better performance**: Reduced overhead
- ✅ **Cleaner code**: Inline handlers with dependency injection
- ✅ **Native OpenAPI**: Built-in Swagger support with .WithOpenApi()
- ✅ **Route groups**: Better organization by entity
- ✅ **Testability**: Business logic in services, endpoints are thin wrappers

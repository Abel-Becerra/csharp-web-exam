# Code Documentation

This document describes the code structure, architecture decisions, design patterns, and best practices implemented in the solution.

## Architecture Overview

The solution follows **Clean Architecture** principles with clear separation of concerns:

### API Project Structure (Clean Architecture)

```
api/
├── Domain/                    # Core business entities (no dependencies)
│   └── Entities/
│       ├── Category.cs       # Category entity
│       └── Product.cs        # Product entity
├── Application/               # Business logic and abstractions
│   ├── DTOs/                 # Data Transfer Objects
│   │   ├── CategoryDto.cs
│   │   ├── ProductDto.cs
│   │   ├── PaginatedResultDto.cs
│   │   └── ProductGroupDto.cs
│   ├── Interfaces/           # Repository and service contracts
│   │   ├── ICategoryRepository.cs
│   │   ├── IProductRepository.cs
│   │   ├── ICategoryService.cs
│   │   └── IProductService.cs
│   └── Services/             # Business logic implementation
│       ├── CategoryService.cs
│       └── ProductService.cs
├── Infrastructure/            # External concerns (data access, logging)
│   ├── Data/
│   │   ├── IDbConnectionFactory.cs
│   │   ├── SqliteConnectionFactory.cs
│   │   └── DbInitializer.cs
│   └── Repositories/         # Dapper-based data access
│       ├── CategoryRepository.cs
│       └── ProductRepository.cs
└── API/                       # Presentation layer
    ├── Endpoints/
    │   ├── CategoryEndpoints.cs
    │   └── ProductEndpoints.cs
    └── Middleware/
        └── ExceptionHandlingMiddleware.cs
```

**Dependency Flow**: API → Application → Domain ← Infrastructure

### UI Project Structure (MVC Pattern)

```
ui/
├── Controllers/              # MVC Controllers
│   ├── HomeController.cs
│   ├── ProductsController.cs
│   └── CategoriesController.cs
├── Models/                   # ViewModels
│   ├── CategoryViewModel.cs
│   └── ProductViewModel.cs
├── Views/                    # Razor views
│   ├── Products/
│   │   ├── Index.cshtml     # List with pagination/filters
│   │   ├── Grouped.cshtml   # Grouped report
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Details.cshtml
│   │   └── Delete.cshtml
│   ├── Categories/          # Category CRUD views
│   └── Shared/
│       └── _Layout.cshtml   # Master layout
└── Services/
    └── ApiClient.cs         # HTTP client for API consumption
```

## Design Patterns and Practices

### 1. Clean Architecture
- **Domain Layer**: Pure business entities with no external dependencies
- **Application Layer**: Business logic, DTOs, and interfaces (depends only on Domain)
- **Infrastructure Layer**: Data access implementation with Dapper (depends on Application)
- **API Layer**: Minimal API endpoints and middleware (depends on Application)

### 2. Repository Pattern
- Abstracts data access logic behind interfaces (`ICategoryRepository`, `IProductRepository`)
- Concrete implementations use Dapper for SQL execution
- Enables easy testing with mocks and future database changes

### 3. Service Layer Pattern
- Business logic encapsulated in service classes (`CategoryService`, `ProductService`)
- Services coordinate between repositories and controllers
- Handles validation, mapping, and business rules

### 4. Dependency Injection (DI)
- All dependencies registered in `Program.cs`
- Constructor injection used throughout
- Promotes loose coupling and testability

```csharp
// Example DI registration
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
```

### 5. DTO Pattern
- Separate DTOs for API requests/responses
- Prevents over-posting and under-posting vulnerabilities
- Clear contract between API and clients

### 6. Factory Pattern
- `IDbConnectionFactory` creates database connections
- Centralizes connection string management
- Facilitates connection pooling and testing

## ORM Implementation

### Dapper (Micro-ORM)
- **Choice rationale**: Lightweight, fast, full SQL control
- **Usage**: Raw SQL queries with parameter binding
- **Benefits**: Performance, simplicity, no magic

```csharp
// Example Dapper query
const string sql = "SELECT Id, Name, Price FROM Products WHERE CategoryId = @CategoryId";
var products = await connection.QueryAsync<Product>(sql, new { CategoryId = categoryId });
```

### Database Access
- **Connection management**: Factory pattern with `IDbConnectionFactory`
- **Transaction handling**: Implicit in Dapper (auto-commit)
- **SQL injection prevention**: Parameterized queries throughout

## Code Quality Standards

### 1. No Hardcoded Values
- Connection strings in `appsettings.json`
- API URLs in `Web.config`
- Magic numbers replaced with named constants
- Configuration-driven behavior

### 2. Consistent Naming Conventions
- **Classes**: PascalCase (`ProductService`, `CategoryRepository`)
- **Methods**: PascalCase (`GetProductByIdAsync`)
- **Variables**: camelCase (`categoryId`, `productName`)
- **Private fields**: `_camelCase` with underscore prefix
- **Interfaces**: IPascalCase (`IProductRepository`)
- **Async methods**: Suffix with `Async`

### 3. Access Modifiers
- **Public**: Only for API contracts and entry points
- **Private**: Internal implementation details
- **Internal**: Infrastructure implementations (if needed)
- Principle of least privilege applied throughout

### 4. Exception Handling
- **Global middleware**: `ExceptionHandlingMiddleware` catches unhandled exceptions
- **Specific catches**: Services catch and log specific exceptions
- **User-friendly messages**: API returns `ProblemDetails` format
- **Logging**: All exceptions logged with context

```csharp
try
{
    // Operation
}
catch (Exception ex)
{
    _log.Error("Context-specific error message", ex);
    throw; // Re-throw for middleware to handle
}
```

### 5. Logging (Log4net)
- **Levels used**: DEBUG, INFO, WARN, ERROR
- **Structured logging**: Consistent message format
- **Context included**: Operation details, IDs, parameters
- **Performance**: Async file appenders

```csharp
_log.Info($"Creating product: {productName}");
_log.Error($"Failed to update product ID {id}", exception);
```

### 6. Validation
- **Client-side**: jQuery Unobtrusive Validation in UI
- **Server-side**: DataAnnotations on DTOs
- **Business rules**: Validated in service layer
- **Database constraints**: Foreign keys, NOT NULL

### 7. Async/Await
- All I/O operations are asynchronous
- Prevents thread blocking
- Improves scalability

## API Design

### RESTful Conventions
- **GET**: Retrieve resources
- **POST**: Create new resources
- **PUT**: Update existing resources
- **DELETE**: Remove resources

### Status Codes
- **200 OK**: Successful GET
- **201 Created**: Successful POST with `Location` header
- **204 No Content**: Successful PUT/DELETE
- **400 Bad Request**: Validation errors
- **404 Not Found**: Resource doesn't exist
- **500 Internal Server Error**: Unhandled exceptions

### Endpoint Examples
```
GET    /api/products?page=1&pageSize=10&search=laptop&categoryId=1
GET    /api/products/{id}
POST   /api/products
PUT    /api/products/{id}
DELETE /api/products/{id}
GET    /api/products/grouped
```

## UI Design

### Minimal API Endpoints (API Project)
- **Route Groups**: Organized by entity (/api/categories, /api/products)
- **Inline Handlers**: Lambda functions with dependency injection
- **OpenAPI Integration**: Automatic Swagger documentation with .WithOpenApi()
- **Typed Results**: Results.Ok(), Results.NotFound(), Results.Created()

### MVC Pattern (UI Project)
- **Model**: ViewModels with validation attributes
- **View**: Razor views with strongly-typed models
- **Controller**: Orchestrates API calls and view rendering

### Client-Server Communication
- `ApiClient` service handles all HTTP communication
- Centralized error handling and logging
- Retry logic for transient failures (basic)

### Responsive Design
- Bootstrap 3 framework
- Mobile-first approach
- Accessible navigation

## Testing Strategy

### Unit Tests (xUnit)
- **Service layer**: Business logic validation
- **Endpoint layer**: Tested indirectly through service tests
- **Mocking**: Moq library for dependencies
- **Coverage**: Core CRUD operations and edge cases

```csharp
[Fact]
public async Task CreateProduct_ValidData_ReturnsCreatedProduct()
{
    // Arrange
    var createDto = new CreateProductDto { Name = "Test", Price = 10.00m, CategoryId = 1 };
    
    // Act
    var result = await _service.CreateProductAsync(createDto);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}
```

**Note**: With Minimal API endpoints, we focus on testing the service layer which contains all business logic. Endpoints are thin wrappers that delegate to services.

## Security Considerations

### Implemented
- **SQL injection prevention**: Parameterized queries
- **CSRF protection**: Anti-forgery tokens in forms
- **Input validation**: Client and server-side
- **Error handling**: No sensitive data in error messages
- **Access modifiers**: Proper encapsulation

### Not Implemented (Development Only)
- Authentication/Authorization
- HTTPS enforcement in production
- Rate limiting
- API keys or tokens

## Performance Optimizations

- **Pagination**: Limits data transfer and rendering
- **Indexes**: Database indexes on foreign keys and search fields
- **Async I/O**: Non-blocking operations
- **Connection pooling**: Managed by ADO.NET
- **Minimal DTOs**: Only necessary fields transferred

## Maintainability Features

- **Clear separation of concerns**: Easy to locate and modify code
- **Dependency injection**: Easy to swap implementations
- **Interface-based design**: Testable and flexible
- **Consistent patterns**: Predictable code structure
- **Comprehensive logging**: Debugging and monitoring support
- **Self-documenting code**: Meaningful names and structure
- **XML comments**: On public APIs (controllers, services)

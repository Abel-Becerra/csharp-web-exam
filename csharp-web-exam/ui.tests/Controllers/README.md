# Controller Tests - Important Information

## ⚠️ Integration Tests Notice

Most tests in this directory are **Integration Tests**, not Unit Tests.

### Current Status

- **Total Tests**: 36
- **Unit Tests**: ~17 (AccountController tests that don't call API)
- **Integration Tests**: ~19 (ProductsController, CategoriesController tests)

---

## 🔴 Why Tests Fail Without API

### Problem
Controllers instantiate `ApiClient` directly without dependency injection:

```csharp
public class ProductsController : Controller
{
    private readonly ApiClient _apiClient = new ApiClient();
    
    public async Task<ActionResult> Index()
    {
        var products = await _apiClient.GetProductsAsync(); // ← Calls real API
        // ...
    }
}
```

### Result
When tests run, they try to connect to the real API at `http://127.0.0.1:5001`.

**Error**: `SocketException: No connection could be made because the target machine actively refused it`

---

## ✅ How to Run Tests

### Option 1: Run with API (Integration Tests)

**Terminal 1** - Start API:
```bash
cd csharp-web-exam/api
dotnet run
```

**Terminal 2** - Run Tests:
```bash
cd csharp-web-exam/ui.tests
dotnet test
```

**Result**: All 36 tests should pass ✅

### Option 2: Run Only Unit Tests

```bash
dotnet test --filter "FullyQualifiedName~AccountControllerTests"
```

**Result**: 10 tests pass ✅ (no API required)

---

## 📊 Test Categories

### AccountControllerTests (10 tests) - ✅ Unit Tests
**No API Required** - These tests work without external dependencies:

- Login_GET_ReturnsViewResult
- Login_GET_WhenAuthenticated_RedirectsToReturnUrl
- Login_POST_WithValidCredentials_RedirectsToReturnUrl
- Login_POST_WithInvalidModel_ReturnsView
- Login_POST_WithInvalidCredentials_ReturnsViewWithError
- Login_POST_WithEmptyUsername_ReturnsViewWithError
- Login_POST_WithEmptyPassword_ReturnsViewWithError
- Logout_ClearsCookieAndSession_RedirectsToLogin
- Logout_WhenNotAuthenticated_StillRedirectsToLogin

### ProductsControllerTests (15 tests) - ⚠️ Integration Tests
**API Required** - These tests call the real API:

- Index (with filters, sorting, pagination)
- Details (GET)
- Create (GET/POST)
- Edit (GET/POST)
- Delete (GET/POST)
- Grouped

### CategoriesControllerTests (12 tests) - ⚠️ Integration Tests
**API Required** - These tests call the real API:

- Index
- Details (GET)
- Create (GET/POST)
- Edit (GET/POST)
- Delete (GET/POST)

---

## 🔧 Future Improvements

### Recommended: Implement Dependency Injection

To create true unit tests, refactor controllers to accept `IApiClient`:

```csharp
// 1. Create interface
public interface IApiClient
{
    Task<List<ProductViewModel>> GetProductsAsync(...);
    Task<ProductViewModel> GetProductByIdAsync(int id);
    // ... other methods
}

// 2. Modify controller
public class ProductsController : Controller
{
    private readonly IApiClient _apiClient;
    
    // Constructor injection
    public ProductsController(IApiClient apiClient = null)
    {
        _apiClient = apiClient ?? new ApiClient();
    }
}

// 3. In tests - Mock the interface
var mockApiClient = new Mock<IApiClient>();
mockApiClient.Setup(x => x.GetProductsAsync(...))
    .ReturnsAsync(new List<ProductViewModel>
    {
        new ProductViewModel { Id = 1, Name = "Test Product" }
    });
    
var controller = new ProductsController(mockApiClient.Object);
var result = await controller.Index();

// Now tests run fast without API! ✅
```

### Benefits of DI Approach:
- ✅ Fast unit tests (no network calls)
- ✅ Reliable tests (no external dependencies)
- ✅ Easy to mock different scenarios
- ✅ Better architecture
- ✅ Follows SOLID principles

---

## 📝 Current Test Approach

### What These Tests Verify

**Integration Tests** verify the complete flow:
1. Controller receives request
2. Controller calls ApiClient
3. ApiClient makes HTTP request to API
4. API processes request
5. Response flows back through layers
6. Controller returns view/redirect

**Value**: These tests catch integration issues and verify the complete system works.

**Limitation**: Require API running, slower, more fragile.

---

## 🎯 Recommendations

### For Development:
1. Keep integration tests for CI/CD pipeline
2. Add unit tests with mocked dependencies
3. Mark integration tests with `[TestCategory("Integration")]`
4. Run unit tests frequently during development
5. Run integration tests before commits

### For CI/CD:
1. Start API in background
2. Run all tests including integration
3. Verify complete system functionality

---

## 📖 Related Documentation

- `Docs/Development/Testing/CONTROLLER_TESTS_ANALYSIS.md` - Detailed analysis
- `Docs/Development/Testing/UI_TESTS_SUMMARY.md` - Complete test summary
- `README.md` (root) - Project documentation

---

**Last Updated**: 2025-10-14  
**Status**: Tests work correctly, require API for integration tests  
**Action**: Consider implementing DI for true unit tests

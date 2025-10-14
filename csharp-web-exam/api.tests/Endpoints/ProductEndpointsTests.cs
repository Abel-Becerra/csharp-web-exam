using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.UseCases.Products;
using Moq;
using Xunit;

namespace api.tests.Endpoints;

public class ProductEndpointsTests
{
    private readonly Mock<IProductService> _mockService;

    public ProductEndpointsTests()
    {
        _mockService = new Mock<IProductService>();
    }
    [Fact]
    public async Task GetAllProducts_ReturnsPagedResults()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new() { Id = 1, Name = "Laptop", Price = 999.99m },
            new() { Id = 2, Name = "Mouse", Price = 29.99m }
        };
        var pagedResult = new PaginatedResultDto<ProductDto>
        {
            Items = products,
            TotalCount = 2,
            Page = 1,
            PageSize = 10
        };
        _mockService.Setup(s => s.GetProductsAsync(1, 10, null, null, null, false)).ReturnsAsync(pagedResult);
        var useCase = new GetProductsUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(1, 10, null, null, null, false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetProductById_ExistingId_ReturnsProduct()
    {
        // Arrange
        var product = new ProductDto { Id = 1, Name = "Laptop", Price = 999.99m };
        _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);
        var useCase = new GetProductByIdUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task GetProductById_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetProductByIdAsync(999)).ReturnsAsync((ProductDto?)null);
        var useCase = new GetProductByIdUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProduct_ValidDto_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "Keyboard", Price = 79.99m, CategoryId = 1 };
        var createdDto = new ProductDto { Id = 1, Name = "Keyboard", Price = 79.99m, CategoryId = 1 };
        _mockService.Setup(s => s.CreateProductAsync(createDto)).ReturnsAsync(createdDto);
        var useCase = new CreateProductUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Keyboard", result.Name);
        Assert.Equal(79.99m, result.Price);
    }

    [Fact]
    public async Task UpdateProduct_ValidDto_ReturnsTrue()
    {
        // Arrange
        var updateDto = new UpdateProductDto { Name = "Updated", Price = 199.99m, CategoryId = 1 };
        _mockService.Setup(s => s.UpdateProductAsync(1, updateDto)).ReturnsAsync(true);
        var useCase = new UpdateProductUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(1, updateDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateProduct_NonExistingId_ReturnsFalse()
    {
        // Arrange
        var updateDto = new UpdateProductDto { Name = "Updated", Price = 199.99m, CategoryId = 1 };
        _mockService.Setup(s => s.UpdateProductAsync(999, updateDto)).ReturnsAsync(false);
        var useCase = new UpdateProductUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(999, updateDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProduct_ExistingId_ReturnsTrue()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(true);
        var useCase = new DeleteProductUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteProduct_NonExistingId_ReturnsFalse()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteProductAsync(999)).ReturnsAsync(false);
        var useCase = new DeleteProductUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetGroupedProducts_ReturnsGroupedData()
    {
        // Arrange
        var groupedData = new List<ProductGroupDto>
        {
            new ProductGroupDto 
            { 
                CategoryName = "Electronics", 
                ProductCount = 5, 
                TotalValue = 2500m, 
                AveragePrice = 500m 
            }
        };
        _mockService.Setup(s => s.GetProductsGroupedByCategoryAsync()).ReturnsAsync(groupedData);
        var useCase = new GetGroupedProductsUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Electronics", result.First().CategoryName);
        Assert.Equal(5, result.First().ProductCount);
    }
}

using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.UseCases.Products;
using Moq;
using Xunit;

namespace api.tests.UseCases.Products;

public class GetProductsUseCaseTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly GetProductsUseCase _useCase;

    public GetProductsUseCaseTests()
    {
        _mockService = new Mock<IProductService>();
        _useCase = new GetProductsUseCase(_mockService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsPagedProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Laptop", Price = 999.99m },
            new ProductDto { Id = 2, Name = "Mouse", Price = 29.99m }
        };
        var pagedResult = new PaginatedResultDto<ProductDto>
        {
            Items = products,
            TotalCount = 2,
            Page = 1,
            PageSize = 10
        };
        _mockService.Setup(s => s.GetProductsAsync(1, 10, null, null, null, false)).ReturnsAsync(pagedResult);

        // Act
        var result = await _useCase.ExecuteAsync(1, 10, null, null, null, false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        _mockService.Verify(s => s.GetProductsAsync(1, 10, null, null, null, false), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithFilters_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 }
        };
        var pagedResult = new PaginatedResultDto<ProductDto>
        {
            Items = products,
            TotalCount = 1,
            Page = 1,
            PageSize = 10
        };
        _mockService.Setup(s => s.GetProductsAsync(1, 10, "Laptop", 1, "price", true)).ReturnsAsync(pagedResult);

        // Act
        var result = await _useCase.ExecuteAsync(1, 10, "Laptop", 1, "price", true);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Laptop", result.Items.First().Name);
    }
}

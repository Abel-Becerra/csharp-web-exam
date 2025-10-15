using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.Services;
using api.Domain.Entities;
using Moq;
using Xunit;

namespace api.tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _service = new ProductService(_mockProductRepository.Object, _mockCategoryRepository.Object);
    }

    [Fact]
    public async Task GetProductsAsync_ReturnsPagedResults()
    {
        // Arrange
        List<Product> products =
        [
            new() { Id = 1, Name = "Product 1", Price = 10.99m, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Product 2", Price = 20.99m, CategoryId = 1, CreatedAt = DateTime.UtcNow }
        ];
        _mockProductRepository.Setup(r => r.GetPagedAsync(1, 10, null, null, null, false))
            .ReturnsAsync((products, 2));

        // Act
        PaginatedResultDto<ProductDto> result = await _service.GetProductsAsync(1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(1, result.Page);
    }

    [Fact]
    public async Task GetProductByIdAsync_ExistingId_ReturnsProduct()
    {
        // Arrange
        Product product = new() 
        { 
            Id = 1, 
            Name = "Test Product", 
            Price = 99.99m, 
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Electronics", CreatedAt = DateTime.UtcNow },
            CreatedAt = DateTime.UtcNow 
        };
        _mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        ProductDto? result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal("Electronics", result.CategoryName);
    }

    [Fact]
    public async Task CreateProductAsync_ValidData_ReturnsCreatedProduct()
    {
        // Arrange
        CreateProductDto createDto = new() { Name = "New Product", Price = 49.99m, CategoryId = 1 };
        Category category = new() { Id = 1, Name = "Electronics", CreatedAt = DateTime.UtcNow };
        
        _mockCategoryRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
        _mockProductRepository.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(1);

        // Act
        ProductDto result = await _service.CreateProductAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(49.99m, result.Price);
    }

    [Fact]
    public async Task CreateProductAsync_InvalidCategoryId_ThrowsException()
    {
        // Arrange
        CreateProductDto createDto = new() { Name = "New Product", Price = 49.99m, CategoryId = 999 };
        _mockCategoryRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateProductAsync(createDto));
    }

    [Fact]
    public async Task UpdateProductAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        Product existingProduct = new() 
        { 
            Id = 1, 
            Name = "Old Name", 
            Price = 10.00m, 
            CategoryId = 1, 
            CreatedAt = DateTime.UtcNow 
        };
        UpdateProductDto updateDto = new() { Name = "Updated Name", Price = 15.00m, CategoryId = 1 };
        
        _mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _mockCategoryRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);

        // Act
        bool result = await _service.UpdateProductAsync(1, updateDto);

        // Assert
        Assert.True(result);
        _mockProductRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Name == "Updated Name" && p.Price == 15.00m)), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        _mockProductRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockProductRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        bool result = await _service.DeleteProductAsync(1);

        // Assert
        Assert.True(result);
        _mockProductRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetProductsGroupedByCategoryAsync_ReturnsGroupedData()
    {
        // Arrange
        var groupedData = new List<(int CategoryId, string CategoryName, int Count, decimal TotalValue, decimal AvgPrice, decimal MinPrice, decimal MaxPrice)>
        {
            (1, "Electronics", 5, 500.00m, 100.00m, 50.00m, 150.00m),
            (2, "Books", 3, 45.00m, 15.00m, 10.00m, 20.00m)
        };
        _mockProductRepository.Setup(r => r.GetGroupedByCategoryAsync()).ReturnsAsync(groupedData);

        // Act
        IEnumerable<ProductGroupDto> result = await _service.GetProductsGroupedByCategoryAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        ProductGroupDto firstGroup = result.First();
        Assert.Equal("Electronics", firstGroup.CategoryName);
        Assert.Equal(5, firstGroup.ProductCount);
    }
}

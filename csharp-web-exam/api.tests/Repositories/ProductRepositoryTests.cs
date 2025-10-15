using api.Application.Interfaces;
using api.Domain.Entities;
using api.Infrastructure.Repositories;
using api.tests.Helpers;
using Microsoft.Data.Sqlite;
using Xunit;

namespace api.tests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IProductRepository _repository;
    private int _categoryId;

    public ProductRepositoryTests()
    {
        // Create in-memory database
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // Create schema
        SqliteCommand createTablesCmd = _connection.CreateCommand();
        createTablesCmd.CommandText = @"
            CREATE TABLE Categories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );
            
            CREATE TABLE Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                CategoryId INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
            );";
        createTablesCmd.ExecuteNonQuery();

        // Insert test category
        SqliteCommand insertCategoryCmd = _connection.CreateCommand();
        insertCategoryCmd.CommandText = "INSERT INTO Categories (Name, CreatedAt) VALUES ('Electronics', @createdAt); SELECT last_insert_rowid();";
        insertCategoryCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));
        _categoryId = Convert.ToInt32(insertCategoryCmd.ExecuteScalar());

        // Create repository
        TestDbConnectionFactory factory = new(_connection);
        _repository = new ProductRepository(factory);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsAllProducts()
    {
        // Arrange
        await InsertTestProduct("Laptop", 999.99m);
        await InsertTestProduct("Mouse", 29.99m);

        // Act
        var (Items, TotalCount) = await _repository.GetPagedAsync(1, 10, null, null, null);

        // Assert
        Assert.Equal(2, TotalCount);
        Assert.Equal(2, Items.Count());
    }

    [Fact]
    public async Task GetPagedAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (int i = 1; i <= 15; i++)
        {
            await InsertTestProduct($"Product {i}", i * 10);
        }

        // Act
        var (Items, TotalCount) = await _repository.GetPagedAsync(2, 5, null, null, null);

        // Assert
        Assert.Equal(15, TotalCount);
        Assert.Equal(5, Items.Count());
    }

    [Fact]
    public async Task GetPagedAsync_WithSearch_ReturnsFilteredResults()
    {
        // Arrange
        await InsertTestProduct("Laptop Dell", 999.99m);
        await InsertTestProduct("Laptop HP", 899.99m);
        await InsertTestProduct("Mouse", 29.99m);

        // Act
        var (Items, TotalCount) = await _repository.GetPagedAsync(1, 10, "Laptop", null, null);

        // Assert
        Assert.Equal(2, TotalCount);
        Assert.All(Items, p => Assert.Contains("Laptop", p.Name));
    }

    [Fact]
    public async Task GetPagedAsync_WithCategoryFilter_ReturnsFilteredResults()
    {
        // Arrange
        await InsertTestProduct("Product 1", 100m);
        await InsertTestProduct("Product 2", 200m);

        // Act
        var (Items, TotalCount) = await _repository.GetPagedAsync(1, 10, null, _categoryId, null);

        // Assert
        Assert.Equal(2, TotalCount);
        Assert.All(Items, p => Assert.Equal(_categoryId, p.CategoryId));
    }

    [Fact]
    public async Task GetPagedAsync_WithSortByName_ReturnsSortedResults()
    {
        // Arrange
        await InsertTestProduct("Zebra", 100m);
        await InsertTestProduct("Apple", 200m);
        await InsertTestProduct("Mango", 150m);

        // Act
        var (Items, TotalCount) = await _repository.GetPagedAsync(1, 10, null, null, "name", false);

        // Assert
        List<string> names = [.. Items.Select(p => p.Name)];
        Assert.Equal("Apple", names[0]);
        Assert.Equal("Mango", names[1]);
        Assert.Equal("Zebra", names[2]);
    }

    [Fact]
    public async Task GetPagedAsync_WithSortByPriceDesc_ReturnsSortedResults()
    {
        // Arrange
        await InsertTestProduct("Product 1", 100m);
        await InsertTestProduct("Product 2", 300m);
        await InsertTestProduct("Product 3", 200m);

        // Act
        var (Items, TotalCount) = await _repository.GetPagedAsync(1, 10, null, null, "price", true);

        // Assert
        List<decimal> prices = [.. Items.Select(p => p.Price)];
        Assert.Equal(300m, prices[0]);
        Assert.Equal(200m, prices[1]);
        Assert.Equal(100m, prices[2]);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
    {
        // Arrange
        int id = await InsertTestProduct("Laptop", 999.99m);

        // Act
        Product? result = await _repository.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal(999.99m, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        Product? result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidProduct_ReturnsId()
    {
        // Arrange
        Product product = new()
        {
            Name = "New Product",
            Price = 499.99m,
            CategoryId = _categoryId,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        int id = await _repository.CreateAsync(product);

        // Assert
        Assert.True(id > 0);
        Product? created = await _repository.GetByIdAsync(id);
        Assert.NotNull(created);
        Assert.Equal("New Product", created.Name);
        Assert.Equal(499.99m, created.Price);
    }

    [Fact]
    public async Task UpdateAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        int id = await InsertTestProduct("Original", 100m);
        Product? product = await _repository.GetByIdAsync(id);
        product!.Name = "Updated";
        product.Price = 200m;
        product.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(product);

        // Assert
        Assert.True(result);
        Product? updated = await _repository.GetByIdAsync(id);
        Assert.Equal("Updated", updated!.Name);
        Assert.Equal(200m, updated.Price);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingProduct_ReturnsFalse()
    {
        // Arrange
        Product? product = new()
        {
            Id = 999,
            Name = "NonExisting",
            Price = 100m,
            CategoryId = _categoryId,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        bool result = await _repository.UpdateAsync(product);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        int id = await InsertTestProduct("ToDelete", 100m);

        // Act
        bool result = await _repository.DeleteAsync(id);

        // Assert
        Assert.True(result);
        Product? deleted = await _repository.GetByIdAsync(id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingProduct_ReturnsFalse()
    {
        // Act
        bool result = await _repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetGroupedByCategoryAsync_ReturnsGroupedData()
    {
        // Arrange
        await InsertTestProduct("Product 1", 100m);
        await InsertTestProduct("Product 2", 200m);
        await InsertTestProduct("Product 3", 150m);

        // Act
        var result = await _repository.GetGroupedByCategoryAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var (CategoryId, CategoryName, Count, TotalValue, AvgPrice, MinPrice, MaxPrice) = result.First();
        Assert.Equal("Electronics", CategoryName);
        Assert.Equal(3, Count);
        Assert.Equal(450m, TotalValue);
        Assert.Equal(150m, AvgPrice);
    }

    private async Task<int> InsertTestProduct(string name, decimal price)
    {
        SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Products (Name, Price, CategoryId, CreatedAt) 
            VALUES (@name, @price, @categoryId, @createdAt); 
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@price", price);
        cmd.Parameters.AddWithValue("@categoryId", _categoryId);
        cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));
        int id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return id;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}

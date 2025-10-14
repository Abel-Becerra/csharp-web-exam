using api.Application.Interfaces;
using api.Domain.Entities;
using api.Infrastructure.Repositories;
using api.tests.Helpers;
using Microsoft.Data.Sqlite;
using Xunit;

namespace api.tests.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ICategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        // Create in-memory database
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // Create schema
        var createTableCmd = _connection.CreateCommand();
        createTableCmd.CommandText = @"
            CREATE TABLE Categories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );";
        createTableCmd.ExecuteNonQuery();

        // Create repository
        var factory = new TestDbConnectionFactory(_connection);
        _repository = new CategoryRepository(factory);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        await InsertTestCategory("Electronics");
        await InsertTestCategory("Books");

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCategory()
    {
        // Arrange
        var id = await InsertTestCategory("Electronics");

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Electronics", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidCategory_ReturnsId()
    {
        // Arrange
        var category = new Category
        {
            Name = "Sports",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var id = await _repository.CreateAsync(category);

        // Assert
        Assert.True(id > 0);
        var created = await _repository.GetByIdAsync(id);
        Assert.NotNull(created);
        Assert.Equal("Sports", created.Name);
    }

    [Fact]
    public async Task UpdateAsync_ExistingCategory_ReturnsTrue()
    {
        // Arrange
        var id = await InsertTestCategory("Electronics");
        var category = await _repository.GetByIdAsync(id);
        category!.Name = "Updated Electronics";
        category.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(category);

        // Assert
        Assert.True(result);
        var updated = await _repository.GetByIdAsync(id);
        Assert.Equal("Updated Electronics", updated!.Name);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingCategory_ReturnsFalse()
    {
        // Arrange
        var category = new Category
        {
            Id = 999,
            Name = "NonExisting",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.UpdateAsync(category);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingCategory_ReturnsTrue()
    {
        // Arrange
        var id = await InsertTestCategory("ToDelete");

        // Act
        var result = await _repository.DeleteAsync(id);

        // Assert
        Assert.True(result);
        var deleted = await _repository.GetByIdAsync(id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingCategory_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    private async Task<int> InsertTestCategory(string name)
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Categories (Name, CreatedAt) VALUES (@name, @createdAt); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));
        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return id;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}

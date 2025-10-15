using api.Application.Interfaces;
using api.Domain.Entities;
using api.Infrastructure.Data;
using Dapper;
using log4net;
using System.Data;

namespace api.Infrastructure.Repositories;

public class CategoryRepository(IDbConnectionFactory connectionFactory) : ICategoryRepository
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(CategoryRepository));
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        const string sql = @"
            SELECT Id, Name, CreatedAt, UpdatedAt 
            FROM Categories 
            ORDER BY Name";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            IEnumerable<Category> categories = await connection.QueryAsync<Category>(sql);
            _log.Debug($"Retrieved {categories.Count()} categories from database");
            return categories;
        }
        catch (Exception ex)
        {
            _log.Error("Database error retrieving all categories", ex);
            throw;
        }
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT Id, Name, CreatedAt, UpdatedAt 
            FROM Categories 
            WHERE Id = @Id";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            Category? category = await connection.QuerySingleOrDefaultAsync<Category>(sql, new { Id = id });
            _log.Debug($"Retrieved category with ID {id}: {(category != null ? "Found" : "Not Found")}");
            return category;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error retrieving category with ID {id}", ex);
            throw;
        }
    }

    public async Task<int> CreateAsync(Category category)
    {
        const string sql = @"
            INSERT INTO Categories (Name, CreatedAt) 
            VALUES (@Name, @CreatedAt);
            SELECT last_insert_rowid();";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            int id = await connection.ExecuteScalarAsync<int>(sql, category);
            _log.Debug($"Created category with ID {id}");
            return id;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error creating category: {category.Name}", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        const string sql = @"
            UPDATE Categories 
            SET Name = @Name, UpdatedAt = @UpdatedAt 
            WHERE Id = @Id";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(sql, category);
            _log.Debug($"Updated category with ID {category.Id}: {rowsAffected} rows affected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error updating category with ID {category.Id}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Categories WHERE Id = @Id";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            _log.Debug($"Deleted category with ID {id}: {rowsAffected} rows affected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error deleting category with ID {id}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM Categories WHERE Id = @Id";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            int count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error checking existence of category with ID {id}", ex);
            throw;
        }
    }
}
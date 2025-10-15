using api.Application.Interfaces;
using api.Domain.Entities;
using api.Infrastructure.Data;
using Dapper;
using log4net;
using System.Data;

namespace api.Infrastructure.Repositories;

public class ProductRepository(IDbConnectionFactory connectionFactory) : IProductRepository
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(ProductRepository));
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        int? categoryId = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        List<string> whereConditions = [];
        DynamicParameters parameters = new();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            whereConditions.Add("p.Name LIKE @SearchTerm");
            parameters.Add("SearchTerm", $"%{searchTerm}%");
        }

        if (categoryId.HasValue)
        {
            whereConditions.Add("p.CategoryId = @CategoryId");
            parameters.Add("CategoryId", categoryId.Value);
        }

        string whereClause = whereConditions.Any()
            ? "WHERE " + string.Join(" AND ", whereConditions)
            : string.Empty;

        // Determine sort column
        string sortColumn = sortBy?.ToLower() switch
        {
            "name" => "p.Name",
            "price" => "p.Price",
            "category" => "c.Name",
            "createdat" => "p.CreatedAt",
            _ => "p.Id"
        };

        string sortDirection = sortDescending ? "DESC" : "ASC";

        // Count query
        string countSql = $@"
            SELECT COUNT(1) 
            FROM Products p 
            {whereClause}";

        // Data query with pagination
        int offset = (page - 1) * pageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", pageSize);

        string dataSql = $@"
            SELECT p.Id, p.Name, p.Price, p.CategoryId, p.CreatedAt, p.UpdatedAt,
                   c.Id, c.Name, c.CreatedAt, c.UpdatedAt
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            {whereClause}
            ORDER BY {sortColumn} {sortDirection}
            LIMIT @PageSize OFFSET @Offset";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            
            int totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            IEnumerable<Product> products = await connection.QueryAsync<Product, Category, Product>(
                dataSql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                parameters,
                splitOn: "Id");

            _log.Debug($"Retrieved {products.Count()} products out of {totalCount} total (Page: {page}, PageSize: {pageSize})");
            
            return (products, totalCount);
        }
        catch (Exception ex)
        {
            _log.Error("Database error retrieving paged products", ex);
            throw;
        }
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT p.Id, p.Name, p.Price, p.CategoryId, p.CreatedAt, p.UpdatedAt,
                   c.Id, c.Name, c.CreatedAt, c.UpdatedAt
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.Id = @Id";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();

            IEnumerable<Product> products = await connection.QueryAsync<Product, Category, Product>(
                sql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new { Id = id },
                splitOn: "Id");

            Product? product = products.FirstOrDefault();
            _log.Debug($"Retrieved product with ID {id}: {(product != null ? "Found" : "Not Found")}");
            return product;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error retrieving product with ID {id}", ex);
            throw;
        }
    }

    public async Task<int> CreateAsync(Product product)
    {
        const string sql = @"
            INSERT INTO Products (Name, Price, CategoryId, CreatedAt) 
            VALUES (@Name, @Price, @CategoryId, @CreatedAt);
            SELECT last_insert_rowid();";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            int id = await connection.ExecuteScalarAsync<int>(sql, product);
            _log.Debug($"Created product with ID {id}");
            return id;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error creating product: {product.Name}", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        const string sql = @"
            UPDATE Products 
            SET Name = @Name, Price = @Price, CategoryId = @CategoryId, UpdatedAt = @UpdatedAt 
            WHERE Id = @Id";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(sql, product);
            _log.Debug($"Updated product with ID {product.Id}: {rowsAffected} rows affected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error updating product with ID {product.Id}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Products WHERE Id = @Id";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            _log.Debug($"Deleted product with ID {id}: {rowsAffected} rows affected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error deleting product with ID {id}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM Products WHERE Id = @Id";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            int count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        catch (Exception ex)
        {
            _log.Error($"Database error checking existence of product with ID {id}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<(int CategoryId, string CategoryName, int Count, decimal TotalValue, decimal AvgPrice, decimal MinPrice, decimal MaxPrice)>> GetGroupedByCategoryAsync()
    {
        const string sql = @"
            SELECT 
                c.Id AS CategoryId,
                c.Name AS CategoryName,
                COUNT(p.Id) AS Count,
                COALESCE(SUM(p.Price), 0) AS TotalValue,
                COALESCE(AVG(p.Price), 0) AS AvgPrice,
                COALESCE(MIN(p.Price), 0) AS MinPrice,
                COALESCE(MAX(p.Price), 0) AS MaxPrice
            FROM Categories c
            LEFT JOIN Products p ON c.Id = p.CategoryId
            GROUP BY c.Id, c.Name
            ORDER BY c.Name";

        try
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            IEnumerable<(int, string, int, decimal, decimal, decimal, decimal)> results = await connection.QueryAsync<(int, string, int, decimal, decimal, decimal, decimal)>(sql);
            _log.Debug($"Retrieved {results.Count()} category groups");
            return results;
        }
        catch (Exception ex)
        {
            _log.Error("Database error retrieving grouped products", ex);
            throw;
        }
    }
}
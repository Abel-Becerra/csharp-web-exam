using Dapper;
using log4net;

namespace api.Infrastructure.Data;

public class DbInitializer(IDbConnectionFactory connectionFactory)
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(DbInitializer));
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    public async Task InitializeAsync()
    {
        _log.Info("Initializing database...");

        try
        {
            // Ensure database directory exists
            EnsureDatabaseDirectoryExists();

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            // Create Users table
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Role TEXT NOT NULL DEFAULT 'User',
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT
                )");

            // Create Categories table
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT
                )");

            // Create Products table
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    CategoryId INTEGER NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT,
                    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
                )");

            // Create indexes for better performance
            await connection.ExecuteAsync(@"
                CREATE INDEX IF NOT EXISTS idx_products_categoryid 
                ON Products(CategoryId)");

            await connection.ExecuteAsync(@"
                CREATE INDEX IF NOT EXISTS idx_products_name 
                ON Products(Name)");

            _log.Info("Database tables created successfully");

            // Seed data if tables are empty
            await SeedDataAsync(connection);
        }
        catch (Exception ex)
        {
            _log.Error("Error initializing database", ex);
            throw;
        }
    }

    private static async Task SeedDataAsync(System.Data.IDbConnection connection)
    {
        var categoryCount = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Categories");

        if (categoryCount > 0)
        {
            _log.Info("Database already contains data, skipping seed");
            return;
        }

        _log.Info("Seeding database with initial data...");

        var now = DateTime.UtcNow.ToString("o");

        // Seed Users (password is "SampleEx4mF0rT3st!ñ" for all)
        var passwordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("SampleEx4mF0rT3st!ñ")));
        
        await connection.ExecuteAsync(@"
            INSERT INTO Users (Username, PasswordHash, Email, Role, CreatedAt) VALUES 
            ('admin', @PasswordHash, 'admin@example.com', 'Admin', @Now),
            ('user1', @PasswordHash, 'user1@example.com', 'User', @Now),
            ('user2', @PasswordHash, 'user2@example.com', 'User', @Now)", 
            new { PasswordHash = passwordHash, Now = now });

        // Seed Categories
        await connection.ExecuteAsync(@"
            INSERT INTO Categories (Name, CreatedAt) VALUES 
            ('Electronics', @Now),
            ('Books', @Now),
            ('Clothing', @Now),
            ('Home & Garden', @Now),
            ('Sports', @Now)", new { Now = now });

        // Seed Products
        await connection.ExecuteAsync(@"
            INSERT INTO Products (Name, Price, CategoryId, CreatedAt) VALUES 
            ('Laptop', 999.99, 1, @Now),
            ('Smartphone', 699.99, 1, @Now),
            ('Wireless Mouse', 29.99, 1, @Now),
            ('USB-C Cable', 12.99, 1, @Now),
            ('The Great Gatsby', 14.99, 2, @Now),
            ('1984', 13.99, 2, @Now),
            ('To Kill a Mockingbird', 15.99, 2, @Now),
            ('T-Shirt', 19.99, 3, @Now),
            ('Jeans', 49.99, 3, @Now),
            ('Sneakers', 79.99, 3, @Now),
            ('Garden Hose', 24.99, 4, @Now),
            ('Plant Pot', 9.99, 4, @Now),
            ('Basketball', 29.99, 5, @Now),
            ('Tennis Racket', 89.99, 5, @Now),
            ('Yoga Mat', 34.99, 5, @Now)", new { Now = now });

        _log.Info("Database seeded successfully with sample data");
    }

    private static void EnsureDatabaseDirectoryExists()
    {
        // Get the connection string to extract the database path
        var appDataPath = Path.Combine(Directory.GetCurrentDirectory(), "app_data");
        
        if (!Directory.Exists(appDataPath))
        {
            _log.Info($"Creating database directory: {appDataPath}");
            Directory.CreateDirectory(appDataPath);
            _log.Info("Database directory created successfully");
        }
    }
}

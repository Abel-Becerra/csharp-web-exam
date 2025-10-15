using api.Application.Interfaces;
using api.Domain.Entities;
using api.Infrastructure.Data;
using Dapper;
using log4net;
using System.Data;

namespace api.Infrastructure.Repositories;

public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(UserRepository));
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    public async Task<User?> GetByUsernameAsync(string username)
    {
        _log.Info($"Getting user by username: {username}");

        using IDbConnection connection = _connectionFactory.CreateConnection();
        
        User? user = await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT Id, Username, PasswordHash, Email, Role, CreatedAt FROM Users WHERE Username = @Username",
            new { Username = username });

        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        _log.Info($"Getting user by ID: {id}");

        using IDbConnection connection = _connectionFactory.CreateConnection();
        
        User? user = await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT Id, Username, PasswordHash, Email, Role, CreatedAt FROM Users WHERE Id = @Id",
            new { Id = id });

        return user;
    }

    public async Task<User> CreateAsync(User user)
    {
        _log.Info($"Creating user: {user.Username}");

        using IDbConnection connection = _connectionFactory.CreateConnection();
        
        string sql = @"
            INSERT INTO Users (Username, PasswordHash, Email, Role, CreatedAt)
            VALUES (@Username, @PasswordHash, @Email, @Role, @CreatedAt);
            SELECT last_insert_rowid();";

        int id = await connection.ExecuteScalarAsync<int>(sql, user);
        user.Id = id;

        _log.Info($"User created with ID: {id}");
        return user;
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        _log.Info($"Checking if user exists: {username}");

        using IDbConnection connection = _connectionFactory.CreateConnection();
        
        int count = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Users WHERE Username = @Username",
            new { Username = username });

        return count > 0;
    }
}
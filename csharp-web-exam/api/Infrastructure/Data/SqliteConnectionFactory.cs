using Microsoft.Data.Sqlite;
using System.Data;

namespace api.Infrastructure.Data;

public class SqliteConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}

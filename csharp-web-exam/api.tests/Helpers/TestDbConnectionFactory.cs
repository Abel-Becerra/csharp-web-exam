using api.Infrastructure.Data;
using Microsoft.Data.Sqlite;

namespace api.tests.Helpers;

/// <summary>
/// Factory for creating test database connections that won't be closed by using statements.
/// This is necessary for SQLite in-memory databases which lose their data when the connection closes.
/// </summary>
public class TestDbConnectionFactory : IDbConnectionFactory
{
    private readonly SqliteConnection _connection;

    public TestDbConnectionFactory(SqliteConnection connection)
    {
        _connection = connection;
    }

    public System.Data.IDbConnection CreateConnection()
    {
        // Return a wrapper that prevents disposal of the shared connection
        return new NonClosingConnectionWrapper(_connection);
    }
}

/// <summary>
/// Wrapper to prevent closing the shared in-memory connection.
/// This allows the repository's using statements to work without actually closing the connection.
/// </summary>
public class NonClosingConnectionWrapper : System.Data.IDbConnection
{
    private readonly SqliteConnection _innerConnection;

    public NonClosingConnectionWrapper(SqliteConnection innerConnection)
    {
        _innerConnection = innerConnection;
    }

    public string ConnectionString
    {
        get => _innerConnection.ConnectionString;
        set => _innerConnection.ConnectionString = value;
    }

    public int ConnectionTimeout => _innerConnection.ConnectionTimeout;
    public string Database => _innerConnection.Database;
    public System.Data.ConnectionState State => _innerConnection.State;

    public System.Data.IDbTransaction BeginTransaction() => _innerConnection.BeginTransaction();
    public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il) => _innerConnection.BeginTransaction(il);
    public void ChangeDatabase(string databaseName) => _innerConnection.ChangeDatabase(databaseName);
    
    // These methods do nothing to prevent closing the shared connection
    public void Close() { /* Do nothing - prevent closing */ }
    public void Dispose() { /* Do nothing - prevent disposal */ }
    
    public System.Data.IDbCommand CreateCommand() => _innerConnection.CreateCommand();
    public void Open()
    {
        // Only open if not already open
        if (_innerConnection.State != System.Data.ConnectionState.Open)
        {
            _innerConnection.Open();
        }
    }
}

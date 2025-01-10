using System.Data;
using Npgsql;

namespace Pricing.Infrastructure;

public class NpqSqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpqSqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
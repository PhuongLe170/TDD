using System.Data;
using Testcontainers.PostgreSql;

namespace Pricing.Infrastructure.Tests;

public class PostgreSqlFixture : IAsyncLifetime
{
    public IDbConnectionFactory ConnectionFactory;
    
    private PostgreSqlContainer  _postgreSqlContainer = new PostgreSqlBuilder()
        .WithDatabase("pricing")
        .Build();
    
    
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        ConnectionFactory = new NpqSqlConnectionFactory(_postgreSqlContainer.GetConnectionString());

        await new DatabaseInitializer(ConnectionFactory).InitializeAsync();
    }

    public Task DisposeAsync()
    {
       return _postgreSqlContainer.DisposeAsync().AsTask();
    }
}
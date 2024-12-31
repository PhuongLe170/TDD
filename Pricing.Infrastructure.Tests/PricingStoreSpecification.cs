using Dapper;
using FluentAssertions;
using Newtonsoft.Json;
using Pricing.Core;
using Pricing.Core.Domain;

namespace Pricing.Infrastructure.Tests;

public class PricingStoreSpecification : IClassFixture<PostgreSqlFixture>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PricingStoreSpecification(PostgreSqlFixture fixture)
    {
        _connectionFactory = fixture.ConnectionFactory;
    }

    [Fact]
    public void Should_throw_argument_null_exception_if_missing_connection_string()
    {
        var create = () => new PostgresPricingStore(null!);

        create.Should().ThrowExactly<ArgumentNullException>();
    }
    
    [Fact]
    public async Task Should_return_true_when_save_with_success()
    {
        IPricingStore store = new PostgresPricingStore(_connectionFactory);
        var pricingTable = CreatePricingTable();

        var result = await store.SaveAsync(pricingTable, CancellationToken.None);
        
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Should_insert_if_not_exists()
    {
        IPricingStore store = new PostgresPricingStore(_connectionFactory);
        var pricingTable = CreatePricingTable();
        await ClearUpPricingStore();

        var result = await store.SaveAsync(pricingTable, CancellationToken.None);
        
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task Should_replace_if_already_exists()
    {
        IPricingStore store = new PostgresPricingStore(_connectionFactory);
        await store.SaveAsync(CreatePricingTable(), default);
        var newPricingTable = new PricingTable(new[]
        {
            new PriceTier(24, 21)
        });

        var result = await store.SaveAsync(newPricingTable, default);

        result.Should().BeTrue();
        var data = await GetPricingFromStore();
        data.Should().HaveCount(1)
            .And
            .Subject
            .First().document.Equals(JsonConvert.SerializeObject(newPricingTable));
    }
    

    private static PricingTable CreatePricingTable()
    {
        return new PricingTable([new PriceTier(24, 1)]);
    }

    private async Task ClearUpPricingStore()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("truncate table Pricing;");
    }

    private async Task<IEnumerable<dynamic>> GetPricingFromStore()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
       // var data = await connection.QueryAsync(@"select * from pricing");
        
        var data = await connection.QueryAsync(
            @"SELECT * FROM pricing;");
        return data;
      //  return data;
    }
}
using Dapper;
using FluentAssertions;
using Newtonsoft.Json;
using Pricing.Core;
using Pricing.Core.ApplyPricing;
using Pricing.Core.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Pricing.Infrastructure.Tests;

public class PricingStoreSpecification : IClassFixture<PostgreSqlFixture>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public PricingStoreSpecification(PostgreSqlFixture fixture)
    {
        _dbConnectionFactory = fixture.ConnectionFactory;
    }

    [Fact]
    public void Should_throw_argument_null_exception_if_missing_connection_string()
    {
        var create = () => new PostgrePricingStore(null);

        create.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public async Task Should_return_true_if_save_succeeded()
    {
        var pricingStore = new PostgrePricingStore(_dbConnectionFactory);
        var pricingTable = CreatePricingTable();

        var result = await pricingStore.SaveAsync(pricingTable, default!);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Should_insert_if_not_exists()
    {
        var pricingStore = new PostgrePricingStore(_dbConnectionFactory);
        var pricingTable = CreatePricingTable();
        await CleanPricingStore();

        var result = await pricingStore.SaveAsync(pricingTable, default!);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Should_replace_if_already_exists()
    {
        IPricingStore store = new PostgrePricingStore(_dbConnectionFactory);
        await store.SaveAsync(CreatePricingTable(), default!);
        var newPricingTable = new PricingTable(new[] { new PriceTier(24, 21) });

        var result = await store.SaveAsync(newPricingTable, default!);
        var data = await GetPricingFromStore();

        result.Should().BeTrue();
        data.Should().HaveCount(1)
            .And
            .Subject
            .First().document.Equals(JsonSerializer.Serialize(newPricingTable));
    }

    private async Task<IEnumerable<dynamic>> GetPricingFromStore()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var data = await connection.QueryAsync(@"select * from pricing;");
        return data;
    }

    private async Task CleanPricingStore()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("truncate table pricing;");
    }

    private static PricingTable CreatePricingTable()
    {
        return new PricingTable(new[] { new PriceTier(24, 1) });
    }
}
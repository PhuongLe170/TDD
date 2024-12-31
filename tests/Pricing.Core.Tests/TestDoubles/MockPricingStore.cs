using FluentAssertions;
using Pricing.Core.Domain;

namespace Pricing.Core.Tests.TestDoubles;

public class MockPricingStore : IPricingStore
{
    private PricingTable _expectedPricingData;

    private PricingTable _savedPricingData;

    public Task<bool> SaveAsync(PricingTable pricingTable, CancellationToken cancellationToken)
    {
        _savedPricingData = pricingTable;
        return Task.FromResult(true);
    }

    public void ExpectedToSave(PricingTable expectedPricingData)
    {
        _expectedPricingData = expectedPricingData;
    }

    public void Verify()
    {
        _savedPricingData.Should().BeEquivalentTo(_expectedPricingData);
    }
}
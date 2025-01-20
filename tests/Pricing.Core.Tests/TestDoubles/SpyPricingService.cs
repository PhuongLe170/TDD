using Pricing.Core.ApplyPricing;
using Pricing.Core.Domain;

namespace Pricing.Core.Tests.TestDoubles;

public class SpyPricingService : IPricingStore
{
    public Task<bool> SaveAsync(PricingTable pricingTable, CancellationToken cancellationToken)
    {
        NumbersOfSaves++;
        return Task.FromResult(true);
    }
    
    public int NumbersOfSaves { get; private set; }
}
using Pricing.Core.ApplyPricing;
using Pricing.Core.Domain;

namespace Pricing.Core.Extensions;

public static class RequestMapToDomain
{
    public static PricingTable ToPricingTable(this ApplyPricingRequest request)
    {
        return new PricingTable(
            request.Tiers.Select(tier => new PriceTier(tier.HourLimit, tier.Price))
        );
    }
}
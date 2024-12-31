using Pricing.Core.Domain;

namespace Pricing.Core;

public record ApplyPricingRequest(IReadOnlyCollection<PriceTierRequest> Tiers);

public record PriceTierRequest(int HourLimit, decimal Price);

using FluentAssertions;
using Pricing.Core.Domain;
using Pricing.Core.Domain.Exceptions;

namespace Pricing.Core.Tests.Domain;

public class PricingTierSpecification
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(25)]
    public void Should_throw_invalid_pricing_tier_when_hour_limit_is_invalid(int hour)
    {
        var create = () => new PriceTier(hour, 1);

        create.Should().ThrowExactly<InvalidPricingTierException>();
    }

    [Fact]
    public void Should_throw_invalid_pricing_tier_exception_when_price_is_negative()
    {
        var create = () => new PriceTier(1, -1);

        create.Should().Throw<InvalidPricingTierException>();
    }
    

    [Theory]
    [InlineData(5, 2, 10)]
    [InlineData(10, 2, 20)]
    [InlineData(5, 4, 20)]
    public void Should_calculate_the_full_price_tier(int hourLimit, decimal price, decimal expected)
    {
        // Arrange
        var tier = new PriceTier(hourLimit, price);
        // Act
        var fullPrice = tier.CalculateFullPrice();
        // Assert
        fullPrice.Should().Be(expected);
    }
}
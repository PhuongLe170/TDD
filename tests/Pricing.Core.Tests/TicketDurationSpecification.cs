using FluentAssertions;
using Pricing.Core.TicketPrice;
using Pricing.Core.TicketPrice.Extenstions;

namespace Pricing.Core.Tests;

public class TicketDurationSpecification
{
    [Fact]
    public void Should_be_1_when_timespan_is_30_minutes()
    {
        var entry = DateTimeOffset.UtcNow;
        var request = new TicketPriceRequest(entry, entry.AddMinutes(30));

        request.GetDurationInHours()
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void Should_be_2_when_timespan_is_2_hours()
    {
        var entry = DateTimeOffset.UtcNow;
        var request = new TicketPriceRequest(entry, entry.AddHours(2));

        request.GetDurationInHours()
            .Should()
            .Be(2);
    }
}
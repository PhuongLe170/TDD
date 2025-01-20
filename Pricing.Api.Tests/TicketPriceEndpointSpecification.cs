using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Pricing.Core;
using Pricing.Core.TicketPrice;

namespace Pricing.Api.Tests;

public class TicketPriceEndpointSpecification
{
    [Fact]
    public async Task Should_return_200_with_pirce_when_get_price()
    {
        var exit = DateTimeOffset.UtcNow;
        var entry = exit.AddMinutes(-30);
        var ticketPriceRequest = new TicketPriceRequest(entry, exit);
        ITicketPriceService ticketPriceService = Substitute.For<ITicketPriceService>();
        ticketPriceService.HandleAsync(ticketPriceRequest, default)
            .Returns(new TicketPriceResponse(2));


        var result = await TicketPriceEndpoint.HandleAsync(
            entry,
            exit,
            ticketPriceService,
            default);
        result.Should().BeOfType<Ok<TicketPriceResponse>>();
        result.Value.Price.Should().Be(2);
        await ticketPriceService.Received(1)
            .HandleAsync(ticketPriceRequest, default);
    }
}
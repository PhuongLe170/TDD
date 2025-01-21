using Pricing.Core.Domain;
using Pricing.Core.TicketPrice.Extenstions;

namespace Pricing.Core.TicketPrice;

public class PriceCalculator : IPriceCalculator
{
    public decimal Calculate(PricingTable pricingTable, TicketPriceRequest ticketPriceRequest)
    {
        var price = 0m;
        var ticketHoursToPay = ticketPriceRequest.GetDurationInHours();

        foreach (var tier in pricingTable.Tiers)
        {
            price += CalculateTierPrice(tier, ticketHoursToPay, price);
            ticketHoursToPay -= tier.HourLimit;

            if (ticketHoursToPay <= 0) break;
        }

        return price;
    }

    private static decimal CalculateTierPrice(PriceTier tier, int ticketHoursToPay, decimal price)
    {
        var hoursInTier = Math.Min(tier.HourLimit, ticketHoursToPay);
        return tier.Price * hoursInTier;
    }
}
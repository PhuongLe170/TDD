namespace Pricing.Core.TicketPrice;

public class TicketPriceService : ITicketPriceService
{
    private readonly IReadPricingStore _pricingStore;
    private readonly IPriceCalculator _priceCalculator;

    public TicketPriceService(IReadPricingStore pricingStore, IPriceCalculator priceCalculator)
    {
        _pricingStore = pricingStore;
        _priceCalculator = priceCalculator;
    }

    public async Task<TicketPriceResponse> HandleAsync(TicketPriceRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Entry >= request.Exit)
        {
            throw new ArgumentException();
        }

        var pricingTable = await _pricingStore.GetAsync(cancellationToken);
        var price = _priceCalculator.Calculate(pricingTable, request);

        return await Task.FromResult(new TicketPriceResponse(Price: price));
    }
}
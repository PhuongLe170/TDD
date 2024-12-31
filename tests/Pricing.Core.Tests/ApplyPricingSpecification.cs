using FluentAssertions;
using NSubstitute;
using Pricing.Core.Domain;
using Pricing.Core.Tests.TestDoubles;

namespace Pricing.Core.Tests;

public class ApplyPricingSpecification
{
    private static int _maxHourLimit = 24;
    private static int _expectedPrice = 1;

    [Fact]
    public async Task Should_Throw_Argument_null_exception_if_request_is_null()
    {
        var pricingManager = new PricingManager(new DummyPricingStore());
        var handlerRequest = () => pricingManager.HandleAsync(null, default);

        await handlerRequest.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Should_return_true_if_succeeded()
    {
        var pricingManager = new PricingManager(new StubSuccessPricingStore());
        var handlerRequest = await pricingManager.HandleAsync(CreateRequest(), default);

        handlerRequest.Should().BeTrue();
    }

    [Fact]
    public async Task Should_return_fail_if_fail_to_save()
    {
        var pricingManager = new PricingManager(new StubFailPricingStore());
        var handlerRequest = await pricingManager.HandleAsync(CreateRequest(), default);

        handlerRequest.Should().BeFalse();
    }


    [Fact]
    public async Task Should_save_only_once()
    {
        var spyPricingService = new SpyPricingService();
        var pricingManager = new PricingManager(spyPricingService);

        _ = await pricingManager.HandleAsync(CreateRequest(), default);

        spyPricingService.NumbersOfSaves.Should().Be(_expectedPrice);
    }

    [Fact]
    public async Task Should_save_expected_data()
    {
        var expectedPricingData = new PricingTable(new[] { new PriceTier(_maxHourLimit, _expectedPrice) });
        var mockPricingStore = new MockPricingStore();
        mockPricingStore.ExpectedToSave(expectedPricingData);
        var pricingManager = new PricingManager(mockPricingStore);

        _ = await pricingManager.HandleAsync(CreateRequest(), default);

        mockPricingStore.Verify();
    }

    [Fact]
    public async Task Should_save_expected_data_nsubstitute()
    {
        var expectedPricingData = new PricingTable(new[] { new PriceTier(_maxHourLimit, _expectedPrice) });
        var mockPricingStore = Substitute.For<IPricingStore>();
        var pricingManager = new PricingManager(mockPricingStore);

        _ = await pricingManager.HandleAsync(CreateRequest(), default);

        await mockPricingStore.Received().SaveAsync(Arg.Is<PricingTable>(
            table => table.Tiers.Count == expectedPricingData.Tiers.Count), default);
    }

    [Fact]
    public async Task Should_save_equivalent_data_to_storage()
    {
        var pricingStore = new InMemoryPricingStore();
        var pricingManager = new PricingManager(pricingStore);
        var applyPricingRequest = CreateRequest();

        _ = await pricingManager.HandleAsync(applyPricingRequest, default);

        pricingStore
            .GetPricingTable()
            .Should()
            .BeEquivalentTo(applyPricingRequest);
    }
    
    private static ApplyPricingRequest CreateRequest()
    {
        return new ApplyPricingRequest(new[] { new PriceTierRequest(_maxHourLimit, _expectedPrice) });
    }
}
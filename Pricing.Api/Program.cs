using Pricing.Api;
using Pricing.Core;
using Pricing.Core.ApplyPricing;
using Pricing.Core.Domain.Exceptions;
using Pricing.Core.TicketPrice;
using Pricing.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new NpqSqlConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")!));
builder.Services.AddScoped<IPricingManager, PricingManager>();
builder.Services.AddScoped<IPricingStore, PostgrePricingStore>();
builder.Services.AddScoped<ITicketPriceService, TicketPriceService>();
builder.Services.AddScoped<IPriceCalculator, PriceCalculator>();
builder.Services.AddScoped<IReadPricingStore, PostgresReadPricingStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPut("/PricingTable", async (IPricingManager pricingManager,
    ApplyPricingRequest request,
    CancellationToken token) =>
{
    try
    {
        var results = await pricingManager.HandleAsync(request, token);
        return results ? Results.Ok() : Results.BadRequest();
    }
    catch (InvalidPricingTierException)
    {
        return Results.Problem();
    }
});
app.MapGet("/TicketPrice", TicketPriceEndpoint.HandleAsync);

await InitializeDatabase(app);

app.Run();
return;

Task InitializeDatabase(WebApplication webApplication) =>
    webApplication.Services.GetService<DatabaseInitializer>()?
        .InitializeAsync() ?? Task.CompletedTask;
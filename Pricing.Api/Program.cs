using Pricing.Core;
using Pricing.Core.Domain.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPricingManager, PricingManager>();

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


app.Run();
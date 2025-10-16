using HealthChecks.UI.Client;
using Microservice.BasketWebAPI.Context;
using Microservice.BasketWebAPI.Dtos;
using Microservice.BasketWebAPI.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Registry;
using Polly.Retry;
using Scalar.AspNetCore;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseInMemoryDatabase("MyDb");
});

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.AddHealthChecks();

builder.Services.AddConsulDiscoveryClient();

const string pipelineName = "my-pipeline";

builder.Services.AddResiliencePipeline(pipelineName, configure =>
{
    configure.AddRetry(new RetryStrategyOptions()
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(15),
        BackoffType = DelayBackoffType.Constant
    });
    configure.AddTimeout(TimeSpan.FromSeconds(60));
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("getall", async (
    ApplicationDbContext dbContext,
    IHttpClientFactory httpClientFactory,
    IDiscoveryClient discoveryClient,
    ResiliencePipelineProvider<string> resiliencePipelineProvider,
    CancellationToken cancellationToken) =>
{
    var baskets = await dbContext.Baskets.ToListAsync(cancellationToken);
    baskets.Add(new Basket()
    {
        ProductId = Guid.Parse("226964cf-7796-4c73-ae15-6616dc9d8f00"),
        Quantity = 5
    });

    var pipeline = resiliencePipelineProvider.GetPipeline(pipelineName);

    var services = await pipeline
                   .ExecuteAsync(async callback
                    => await discoveryClient.GetInstancesAsync("Product-WebAPI", cancellationToken));

    var productEndpointDiscovery = services.FirstOrDefault();

    if (productEndpointDiscovery is null)
    {
        return Results.NotFound(productEndpointDiscovery);
    }

    var productEndpoint = productEndpointDiscovery.Uri;

    using var httpClient = httpClientFactory.CreateClient();

    var products = await pipeline
            .ExecuteAsync(async callback
                => await httpClient.GetFromJsonAsync<List<ProductDto>>($"{productEndpoint}getall", cancellationToken));

    if (products is null) return Results.NotFound(products);

    var res = baskets.Select(s => new
    {
        Id = s.Id,
        ProductId = s.ProductId,
        ProductName = products.FirstOrDefault(p => p.Id == s.ProductId)?.Name ?? "",
        Quantity = s.Quantity,
    });

    return Results.Ok(res);
}).Produces<List<Basket>>();

app.MapHealthChecks("health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
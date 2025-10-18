using HealthChecks.UI.Client;
using MassTransit;
using Microservice.BasketWebAPI.Consumer;
using Microservice.BasketWebAPI.Context;
using Microservice.BasketWebAPI.Dtos;
using Microservice.BasketWebAPI.Models;
using Microservice.Shared;
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

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderResultConsumer>();
    x.AddConsumer<ProductResultConsumer>();
    x.UsingRabbitMq((context, configure) =>
    {
        configure.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        configure.ReceiveEndpoint("order-result-basket-endpoint", e =>
        {
            e.ConfigureConsumer<OrderResultConsumer>(context);
            e.ConfigureConsumer<ProductResultConsumer>(context);
        });
    });
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapPost("create-order", async (IPublishEndpoint publishEndpoint, ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
{
    var baskets = await dbContext.Baskets.ToListAsync(cancellationToken);

    //basket þu anda boþ olduðundan bunu elle ürettim
    List<CreateOrderDetailDto> orderDetails = new()
    {
        new CreateOrderDetailDto(Guid.Parse("1d853f04-a795-4d51-b7ca-974777b95284"), 5),
        new CreateOrderDetailDto(Guid.Parse("d4f66a4e-c84b-4e5a-94f7-c0043160d771"), 3),
        new CreateOrderDetailDto(Guid.Parse("5e5b4041-bfa8-498c-a1bf-a86c0b7e2e93"),8),
    };

    CreateOrderDto createOrder = new(orderDetails);

    await publishEndpoint.Publish(createOrder);

    return Results.Ok(new { message = "Sipariþ oluþturuluyor..." });
});

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
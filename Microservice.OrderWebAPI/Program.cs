using MassTransit;
using Microservice.OrderWebAPI.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BasketConsumer>();
    x.UsingRabbitMq((contex, configure) =>
    {
        configure.Host("localhost", "/", h => { });
        configure.ReceiveEndpoint("order-basket-endpoint", e =>
        {
            e.ConfigureConsumer<BasketConsumer>(contex);
        });
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();

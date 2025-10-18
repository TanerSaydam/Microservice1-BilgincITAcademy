using MassTransit;
using Microservice.OrderWebAPI.Consumer;
using Microservice.OrderWebAPI.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("MyDb"));

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

using Microservice.BasketWebAPI.Context;
using Microservice.BasketWebAPI.Dtos;
using Microservice.BasketWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseInMemoryDatabase("MyDb");
});

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("getall", async (ApplicationDbContext dbContext, HttpClient httpClient, CancellationToken cancellationToken) =>
{
    var baskets = await dbContext.Baskets.ToListAsync(cancellationToken);
    baskets.Add(new Basket()
    {
        ProductId = Guid.Parse("226964cf-7796-4c73-ae15-6616dc9d8f00"),
        Quantity = 5
    });

    var products = await httpClient.GetFromJsonAsync<List<ProductDto>>("http://localhost:6001/getall", cancellationToken);

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

app.Run();
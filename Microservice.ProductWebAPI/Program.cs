using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Service Registration

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseInMemoryDatabase("MyDb");
});

//builder.Services.AddSwaggerGen();
//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

//app.UseSwagger();
//app.UseSwaggerUI();

//Middleware

app.MapGet("getall", async (ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
{
    var res = await dbContext.Products.ToListAsync(cancellationToken);
    res.Add(new Product()
    {
        Id = Guid.Parse("226964cf-7796-4c73-ae15-6616dc9d8f00"),
        Name = "Bilgisayar",
        Price = 45000,
        Stock = 10
    });

    return Results.Ok(res);
}).Produces<List<Product>>();

app.Run();
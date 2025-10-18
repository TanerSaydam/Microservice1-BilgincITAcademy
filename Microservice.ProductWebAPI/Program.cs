using Microservice.AuthLayer;
using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

//Service Registration

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseInMemoryDatabase("MyDb");
});

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();
#region MyRegion
builder.Services.AddHealthChecks();
#endregion

builder.Services.AddConsulDiscoveryClient();
builder.Services.AddResponseCompression(x =>
{
    x.EnableForHttps = true;
});

builder.Services.AddControllers();

builder.Services.AddAuthLayer();

//builder.Services.AddOpenTelemetry()
//    .WithTracing(c =>
//    {
//        c.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ProductWebAPI"));
//        c.AddAspNetCoreInstrumentation();
//        c.AddHttpClientInstrumentation();
//        c.AddEntityFrameworkCoreInstrumentation(o =>
//        {
//            o.SetDbStatementForText = true;
//            o.SetDbStatementForStoredProcedure = true;
//            o.EnrichWithIDbCommand = (activity, command) =>
//            {
//            };
//        });
//        c.AddConsoleExporter();
//        c.AddOtlpExporter();
//    });

builder.AddServiceDefaults();

var app = builder.Build();


var connectionstring = builder.Configuration.GetConnectionString("eticaretdb");

Console.WriteLine("ConnectionString: {0}", connectionstring);
//Middleware
app.MapOpenApi();
app.MapScalarApiReference();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("getall", async (ApplicationDbContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
{
    HttpClient httpClient = new();
    await httpClient.GetAsync("https://jsonplaceholder.typicode.com/todos", cancellationToken);

    var res = await dbContext.Products.ToListAsync(cancellationToken);
    res.Add(new Product()
    {
        Id = Guid.Parse("226964cf-7796-4c73-ae15-6616dc9d8f00"),
        Name = "Bilgisayar",
        Price = 45000,
        Stock = 10
    });

    return Results.Ok(res);
})
    .Produces<List<Product>>()
    .RequireAuthorization();

//app.MapHealthChecks("health");
app.UseResponseCompression();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();
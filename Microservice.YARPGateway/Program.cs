using Microservice.AuthLayer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using Yarp.ReverseProxy.Swagger;
using Yarp.ReverseProxy.Swagger.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddSwagger(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("fixed", x =>
    {
        x.PermitLimit = 100;
        x.QueueLimit = 100;
        x.Window = TimeSpan.FromSeconds(1);
        x.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddAuthLayer();
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("require-authentication", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
        options.SwaggerEndpoint("http://localhost:6001/swagger/v1/swagger.json", "ProductWebAPI");
        //foreach (var cluster in config.Clusters)
        //{
        //    options.SwaggerEndpoint($"/swagger/{cluster.Key}/swagger.json", cluster.Key);
        //}
    });
}

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .SetIsOriginAllowed(x => true)
    .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.MapGet("/", () => "Hello World from YARP!");

app.MapReverseProxy();

app.UseRateLimiter();

app.Run();

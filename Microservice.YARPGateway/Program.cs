var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .SetIsOriginAllowed(x => true)
    .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.MapGet("/", () => "Hello World from YARP!");

app.MapReverseProxy();

app.Run();

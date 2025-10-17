using Microservice.AuthWebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<JwtProvider>();

var app = builder.Build();

app.MapGet("/login", (JwtProvider jwtProvider) =>
{
    var token = jwtProvider.CreateToken();
    return Results.Ok(new { Token = token });
});

app.Run();

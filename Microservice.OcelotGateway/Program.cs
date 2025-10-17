using Microservice.AuthLayer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);//.AddConsul().AddPolly();

builder.Services.AddCors();

builder.Services.AddAuthLayer();

var app = builder.Build();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod()
.SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

await app.RunAsync();
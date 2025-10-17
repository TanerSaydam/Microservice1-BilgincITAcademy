using Microservice.HashiCorpVault;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<VaultService>();

var app = builder.Build();

app.MapGet("/get-secrets", async (VaultService vaultService) =>
{
    var secret = await vaultService.GetSecrets("productapp/config");
    Console.WriteLine(secret.Data.Data["ConnectionString"]);
    Console.WriteLine(secret.Data.Data["JWT"]);

    return Results.Ok();
});

app.Run();

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var userName = builder.AddParameter("username", "postgres", true);
var password = builder.AddParameter("passowrd", "Password12*", true);

var postgre = builder.AddPostgres("postgres", userName: userName, password: password, port: 5432)
    .WithLifetime(ContainerLifetime.Persistent);

var eticaretdb = postgre.AddDatabase("eticaretdb");

builder.AddProject<Microservice_ProductWebAPI>("ProductWebAPI")
    .WithReference(eticaretdb)
    .WaitFor(eticaretdb);

builder.AddProject<Microservice_BasketWebAPI>("BasketWebAPI");
builder.AddProject<Microservice_AuthWebAPI>("AuthWebAPI");
builder.AddProject<Microservice_OcelotGateway>("OcelotGateway");
builder.AddProject<Microservice_YARPGateway>("YARPGateway");

builder.AddProject<Microservice_OrderWebAPI>("OrderWebAPI");

builder.Build().Run();
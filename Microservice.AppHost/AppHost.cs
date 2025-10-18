using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region PostgresDb
//var postgresUserName = builder.AddParameter("username", "postgres", true);
//var postgresPassword = builder.AddParameter("passowrd", "Password12*", true);

//var postgre = builder.AddPostgres("postgres", userName: postgresUserName, password: postgresPassword, port: 5432)
//    .WithLifetime(ContainerLifetime.Persistent);

//var eticaretdb = postgre.AddDatabase("eticaretdb");
#endregion

var rabbitMQUserName = builder.AddParameter("username", "guest", true);
var rabbitMQPassword = builder.AddParameter("passowrd", "guest", true);

var rabitmq = builder.AddRabbitMQ("rabbitmq", rabbitMQUserName, rabbitMQPassword, 5672)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImage("rabbitmq", "3-management")
    .WithHttpEndpoint(port: 15672, targetPort: 15672, name: "management");

#region Projects
//parts
var product = builder.AddProject<Microservice_ProductWebAPI>("ProductWebAPI")
    //.WithReference(eticaretdb)
    //.WaitFor(eticaretdb)
    .WaitFor(rabitmq);
var basket = builder.AddProject<Microservice_BasketWebAPI>("BasketWebAPI")
    .WaitFor(rabitmq);
var order = builder.AddProject<Microservice_OrderWebAPI>("OrderWebAPI")
    .WaitFor(rabitmq);

//auth
var auth = builder.AddProject<Microservice_AuthWebAPI>("AuthWebAPI");

//gateway
builder.AddProject<Microservice_OcelotGateway>("OcelotGateway")
    .WaitFor(product)
    .WaitFor(basket)
    .WaitFor(order)
    .WaitFor(auth);
builder.AddProject<Microservice_YARPGateway>("YARPGateway")
    .WaitFor(product)
    .WaitFor(basket)
    .WaitFor(order)
    .WaitFor(auth);
#endregion

builder.Build().Run();
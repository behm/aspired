using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

// NOTE: If you want to decide how to run SQL Server in Production or DEV, you can do something like this
// var sqlServer = builder.ExecutionContext.IsPublishMode
//      ? builder.AddConnectionString("my-server")
//      : builder.AddSqlServer("my-server").AddDatabase("my-db");

// NOTE: coming from the community: MSBuild.Sdk.SqlProj.Aspire (Preview still?)
//builder.AddSqlProject("database") // todo: look into this one!
//    .FromDacpac("database.dacpac")
//    .PublishTo(sqlContainer);


var sqlPassword = builder.AddParameter("sql-password");
var sqlShell = "./SqlServer";
var sqlScript = "../Aspired.Database/sql";

var sqlServer = builder
    .AddSqlServer("aspired-sql", sqlPassword, 7890)
    .WithDataVolume("aspired-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount(sqlShell, target: "/usr/config")                     // NOTE: ensure all script line endings are LF for Linux container
    .WithBindMount(sqlScript, target: "/docker-entrypoint-initdb.d")    // NOTE: ensure all script line endings are LF for Linux container
    //.WithEntrypoint("/usr/config/entrypoint.sh");
    ;
    //.PublishAsConnectionString()  // if you want to just publish this as a connection string
    //.PublishAsAzureSqlDatabase()  // todo: come back to this

var sqlDatabase = sqlServer.AddDatabase("aspired-db", "Aspired");


var apiService = builder.AddProject<Projects.Aspired_ApiService>("apiservice")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);

builder.AddProject<Projects.Aspired_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WaitFor(cache)
    .WaitFor(apiService);

// NOTE: coming soon???  Check this out!  I think it's pretty much an OData endpoint for your database???
//       https://aka.ms/dab/aspire
//var dabConfig = "./dab-config.json";
//var dabServer = builder
//    .AddContainer("data-api", "mcr.microsoft.com/azure-databases/data-api-builder")
//    .WithBindMount(dabConfig, "/App/dab-config.json")
//    .WithHttpEndpoint(5000, 5000, "http")
//    .WithReference(sqlServer);

builder.Build().Run();


// connection string: Data Source-127.0.0.1,1234; Initial Catalog=aspired-db;
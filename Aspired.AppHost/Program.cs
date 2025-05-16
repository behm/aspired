using Aspire.Hosting.Publishing;

using Aspired.AppHost.Commands;

using Microsoft.Extensions.Hosting;

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


var sqlPassword = builder.AddParameter("sql-password", new SqlPasswordDefault());    // password can be overridden in user secrets or environment variables

#pragma warning disable ASPIREPROXYENDPOINTS001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var sqlServer = builder
    .AddSqlServer("aspired-sql", sqlPassword, 7890)
    .WithDataVolume("aspired-data")
    .WithContainerName("aspired-sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false);
    //    .WithBindMount("./SqlServerConfig", target: "/usr/config")                          // NOTE: ensure all script are UTF-8 with LF line endings for Linux container
    //.WithBindMount("../Aspired.Database/sql", target: "/docker-entrypoint-initdb.d")    // NOTE: ensure all script are UTF-8 with LF line endings for Linux container
    //    .WithBindMount("./Database", target: "/docker-entrypoint-initdb.d")                 // NOTE: ensure all script are UTF-8 with LF line endings for Linux container
    //    .WithEntrypoint("/usr/config/entrypoint.sh");

    //.PublishAsConnectionString()  // if you want to just publish this as a connection string
    //.PublishAsAzureSqlDatabase()  // todo: come back to this
#pragma warning restore ASPIREPROXYENDPOINTS001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var sqlDatabase = sqlServer.AddDatabase("aspired-db", "Aspired");
if (builder.Environment.IsDevelopment())
{ 
    sqlDatabase.
        WithResetDatabaseCommand();
}

var dbMigrator = builder.AddProject<Projects.Aspired_DatabaseMigrations>("db-migrator")
    .WithReference(sqlDatabase)
    .WithReference(sqlServer)
    .WaitFor(sqlServer)
    .WithExplicitStart();

var apiService = builder.AddProject<Projects.Aspired_ApiService>("apiservice")
    .WithReference(sqlDatabase)
    .WaitFor(sqlDatabase)
    .WaitForCompletion(dbMigrator);

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

// NOTE: coming soon???  Check this out!  I think it's pretty much an OData endpoint for your database???
//       https://aka.ms/dab/aspire
//var dabConfig = "./dab-config.json";
//var dabServer = builder
//    .AddContainer("data-api", "mcr.microsoft.com/azure-databases/data-api-builder")
//    .WithBindMount(dabConfig, "/App/dab-config.json")
//    .WithHttpEndpoint(5000, 5000, "http")
//    .WithReference(sqlServer);

builder.Build().Run();


internal class SqlPasswordDefault : ParameterDefault
{
    public override string GetDefaultValue()
    {
        return "P@ssw0rd12345";
    }

    public override void WriteToManifest(ManifestPublishingContext context)
    {
    }
}
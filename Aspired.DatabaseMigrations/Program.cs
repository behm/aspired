// See https://aka.ms/new-console-template for more information
using DbUp;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Let's migrate some database changes!");

var builder = Host.CreateApplicationBuilder(args);

builder.AddSqlServerClient("aspired-sql");

var host = builder.Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();

var connectionString = configuration.GetConnectionString("aspired-db");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("No connection string found!");
}

// ============================================================================
// Start database migrations
// ============================================================================
 
EnsureDatabase.For.SqlDatabase(connectionString);

var upgradeEngine = DeployChanges.To
    .SqlDatabase(connectionString)
    .LogToConsole() // todo: figure out better logging solution (i.e. ILogger???)
    .WithScriptsFromFileSystem("Scripts")
    .Build();

var result = upgradeEngine.PerformUpgrade();

if (!result.Successful)
{
    Console.WriteLine("Upgrade FAILED");    // todo: log this
    return;
}

Console.WriteLine("Upgrade was SUCCESSFUL!!!"); // todo: log this

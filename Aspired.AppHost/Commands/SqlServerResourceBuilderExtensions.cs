using Microsoft.Data.SqlClient;

namespace Aspired.AppHost.Commands
{
    public static class SqlServerResourceBuilderExtensions
    {
        public static IResourceBuilder<SqlServerDatabaseResource> WithResetDatabaseCommand(this IResourceBuilder<SqlServerDatabaseResource> builder)
        {
            builder.WithCommand(
                name: "reset-database", 
                displayName: "Reset Database (DROP & RE-CREATE)",
                executeCommand: async (ExecuteCommandContext context) =>
                {
                    var connectionString = await builder.Resource.ConnectionStringExpression.GetValueAsync(context.CancellationToken);
                    var masterConnectionString = new SqlConnectionStringBuilder(connectionString)
                    {
                        InitialCatalog = "master"
                    }.ConnectionString;
                    using var connection = new SqlConnection(masterConnectionString);
                    await connection.OpenAsync(context.CancellationToken);

                    // DROP & recreate the database
                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = $@"
                        ALTER DATABASE [{builder.Resource.DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        IF DB_ID(N'{builder.Resource.DatabaseName}') IS NOT NULL
                        BEGIN
                            DROP DATABASE [{builder.Resource.DatabaseName}];
                        END
                    ";
                        //CREATE DATABASE [{builder.Resource.DatabaseName}];
                    await cmd.ExecuteNonQueryAsync(context.CancellationToken);

                    // Command execution logic here
                    await Task.CompletedTask;
                    return CommandResults.Success();
                },
                commandOptions: new CommandOptions
                {
                    UpdateState = (UpdateCommandStateContext context) =>
                    {
                        // State update logic here
                        return ResourceCommandState.Enabled;
                    },
                    Description = "DROPs the database and recreates it so that migrations can be re-run",
                    Parameter = new[] { "", "" },
                    ConfirmationMessage = "Are you sure?  This will DELETE all data!",
                    IconName = "ArrowReset",
                    IconVariant = IconVariant.Filled,
                    IsHighlighted = false
                });

            return builder;
        }
    }
}

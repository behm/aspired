using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Aspired.AppHost.Commands
{
    public static class SqlServerResourceBuilderExtensions
    {
        public static IResourceBuilder<SqlServerServerResource> WithResetDatabaseCommand(this IResourceBuilder<SqlServerServerResource> builder)
        {
            builder.WithCommand(
                name: "reset-database",
                displayName: "Reset Database",
                executeCommand: context => OnRunResetDatabase(builder, context),
                updateState: OnUpdateResourceState,
                iconName: "AnimalRabbitOff",
                iconVariant: IconVariant.Filled
            );

            return builder;
        }

        public static async Task<ExecuteCommandResult> OnRunResetDatabase(
            IResourceBuilder<SqlServerServerResource> builder, 
            ExecuteCommandContext context)
        {
            //var connectionString = builder.

            //var resource = context.Resource;
            //var connection = resource.GetConnection();
            //// NOTE: This is a simple example of how to reset a database
            ////       You should be careful with this command as it will delete all data in the database
            ////       and recreate the schema
            //await connection.ResetDatabaseAsync();
            
            await Task.Delay(1000);

            return CommandResults.Success();
        }

        private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
        {
            var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Updating resource state: {ResourceSnapshot}", context.ResourceSnapshot);
            }

            return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled;
        }
    }
}

# Aspired

.NET Aspire sample application with several bells and whistles

## NuGet Packages

Add the following packages to the AppHost project

- Aspire.Hosting.SqlServer

## Getting Started

### Secrets

You will need to create a Parameter entry in the secrets.json file to provide the SQL Password used for creating the SQL Server container.

`dotnet user-secrets set "Parameres:SqlPassword" "YourPassword"`

The SQL Server port is hard-coded to `7890` but you could create another parameter and add the corresponding secrets value.


## References

- Followed this tutorial: https://www.youtube.com/watch?v=UW6t9AXF6OA
    - This is pre-.NET 9
- Uses a SQL Project to provide script for database creation
    - https://www.youtube.com/watch?v=tQ9wktm9BQA
- https://www.youtube.com/watch?v=uIN7iEsRkS4
- This tutorial has no sound but shows how to add a SQL Server container and BindMount the scripts to initialize the database
    - https://www.youtube.com/watch?v=h09MfcE9LdI
- [Copy the shell scripts to initialize the database from here](https://github.com/dotnet/aspire-samples/tree/main/samples/DatabaseContainers/DatabaseContainers.AppHost/sqlserverconfig)
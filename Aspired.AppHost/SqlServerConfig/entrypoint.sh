#!/bin/bash

# Copied from: https://github.com/dotnet/aspire-samples/blob/main/samples/DatabaseContainers/DatabaseContainers.AppHost/sqlserverconfig/entrypoint.sh

# Adapted from: https://github.com/microsoft/mssql-docker/blob/80e2a51d0eb1693f2de014fb26d4a414f5a5add5/linux/preview/examples/mssql-customize/entrypoint.sh

# Start the script and create the DB and user
/usr/config/configure-db.sh &

# Start SQL Server
/opt/mssql/bin/sqlservr

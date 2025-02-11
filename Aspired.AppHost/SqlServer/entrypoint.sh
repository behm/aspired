#!/bin/bash

# Adapted from: https://github.com/microsoft/mssql-docker/blob/.../linux/preview/examples/mssql-customize/entrypoint.sh

# Start the script and create the DB and user
/usr/config/configure-db.sh &

# Start SQL Server
/opt/mssql/bin/sqlserver

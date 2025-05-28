# Use the Microsoft SQL Server image as the base
FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables required for SQL Server
ENV ACCEPT_EULA=Y
ENV MSSQL_PID=Express
ENV MSSQL_SA_PASSWORD=Str0ngP@ssw0rd123!

# Switch to root for directory creation and additional steps
USER root
RUN mkdir -p /app/init-scripts

# Copy the initialization script to container
COPY init-database.sql /app/init-scripts/

# Install necessary tools
RUN apt-get update && apt-get install -y mssql-tools unixodbc-dev && apt-get clean

# Revert to default SQL server user for security
USER mssql

# Proper HEALTHCHECK command (with valid spacing)
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
  CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -Q "SELECT 1" || exit 1

# Command to start SQL Server and execute the initialization script
CMD /bin/bash -c "/opt/mssql/bin/sqlservr & sleep 25 && \
  if /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -Q 'SELECT name FROM sys.databases WHERE name = \"AgriEnergyConnectDB\"' | grep -q AgriEnergyConnectDB; then \
    echo 'Database already exists, skipping initialization'; \
  else \
    echo 'Initializing database'; \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -d master -i /app/init-scripts/init-database.sql; \
  fi && wait"

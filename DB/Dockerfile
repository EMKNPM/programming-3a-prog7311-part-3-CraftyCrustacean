FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA=Y
ENV MSSQL_PID=Express
ENV MSSQL_SA_PASSWORD=Qwerty123

USER root
RUN mkdir -p /app/init-scripts

COPY init-database.sql /app/init-scripts/

RUN apt-get update && apt-get install -y mssql-tools unixodbc-dev && apt-get clean

USER mssql

HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
    CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -Q "SELECT 1" || exit 1

CMD /bin/bash -c "/opt/mssql/bin/sqlservr & sleep 25 && \
    if /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -Q \
    'SELECT name FROM sys.databases WHERE name = \"GLMS_Db\"' | grep -q GLMS_Db; then \
        echo 'Database already exists, skipping initialization'; \
    else \
        echo 'Initializing database'; \
        /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -d master -i /app/init-scripts/init-database.sql; \
    fi && wait"
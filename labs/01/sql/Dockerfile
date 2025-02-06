FROM mcr.microsoft.com/mssql/server:latest
ENV SA_PASSWORD=DevPass_1
ENV MSSQL_PID=Express
ENV ACCEPT_EULA=Y

COPY northwind.sql /usr/scripts/
COPY entrypoint.sh /usr/scripts/
COPY init-db.sh /usr/scripts/

EXPOSE 1433
CMD /bin/bash /usr/scripts/entrypoint.sh

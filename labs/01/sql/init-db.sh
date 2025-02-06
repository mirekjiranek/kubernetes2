# wait until SQL Server is alive
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -Q "SELECT 1" -C -l 1; do
  echo "SQL Server not ready yet, will retry..."
  sleep 3
done

# detect whether the DB exists
if ! /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -Q "SELECT name FROM sys.databases" -C | grep -q Northwind
then
  # deploy database
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -C -i /usr/scripts/northwind.sql
  echo "Northwind database initialized successfully."

else
  echo "The Northwind database already exists."
fi


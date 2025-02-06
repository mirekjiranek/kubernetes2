# wait until SQL Server is alive
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -Q "SELECT 1" -l 1; do
  echo "SQL Server not ready yet, will retry..."
  sleep 3
done

# detect whether the DB exists
if ! /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -Q "SELECT name FROM sys.databases" | grep -q Northwind
then
  # deploy database
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /usr/scripts/northwind.sql
  echo "Northwind database initialized successfully."

else
  echo "The Northwind database already exists."
fi


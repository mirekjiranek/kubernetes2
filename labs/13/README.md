# Lab 13

Cílem tohoto labu je nasazení SQL Serveru do clusteru.

## Krok 1 - Příprava a nasazení deploymentu

1. Vytvořte namespace pro SQL Server (můžeme ho chtít sdílet mezi více aplikacemi):

```
kubectl create namespace mssql
```

2. Přepněte si kontext do tohoto namespace pomocí příkazu `kubectl config set-context --current --namespace=mssql`

3. Ujistěte se, že jste v shellu na cestě `labs/13`

4. Nejprve nasaďme persistent volume claim a příslušný volume: `kubectl apply -f sql-volumes.yml`

5. Počkejme, než se volume claim založí. Pole __Status__ musí mít hodnotu __Bound__.

```
kubectl describe pvc mssql-data
```

6. Nyní vytvořme secret, v němž bude uloženo heslo pro účet __sa__:

```
kubectl create secret generic mssql --from-literal=SA_PASSWORD="DevPass_1"
```

7. Nasaďte deployment a službu pro SQL Server: `kubectl apply -f sql-deployment.yml`

> Všimněte si, že nenasazujeme náš kontejner s předpřipravenou databází, ale čistý SQL Server. Databázi v Kubernetes může chtít využívat více aplikací.

8. Ověřte, že služba je nasazená a běží.

9. Pomocí `kubectl port-forward` se k serveru připojte (port 1433 na vašem počítači může být blokován, namapujte si jej tedy třeba takto - `51433:1433`).

10. V SQL Server Management studiu ověřte, že server běží.

> Hostname by měl být `tcp:127.0.0.1,51433`, uživatel `sa`, heslo `DevPass_1`

## Krok 2 - inicializace databáze

1. Spusťte proti serveru skript `labs/13/northwindtest.sql`. Může to chvíli trvat.

2. Dále spusťte příkaz následující příkazy k založení uživatele pro aplikaci:

```
USE [master] 
GO

CREATE LOGIN [NorthwindTestLogin] WITH PASSWORD = N'AppPass_1'
GO

USE [NorthwindTest]
GO

CREATE USER [NorthwindTestUser] FOR LOGIN [NorthwindTestLogin]
GO

EXEC sp_addrolemember N'db_owner', N'NorthwindTestUser'
```

> Aplikace se nebude k databázi přihlašovat účtem __sa__, ale bude mít svůj aplikační účet `NorthwindTestLogin` s heslem `AppPass_1`.

## Krok 3 - úprava deploymentu

Connection string k databázi není rozumné ukládat do YAML souboru Kubernetes - ty chceme mít v gitu, kde by secrety být neměly. Z tohoto důvodu uložíme hodnotu do secrets v Kubernetes.

1. Přepněte se zpět do namespace `northwind-test`:

```
kubectl config set-context --current --namespace=northwind-test
```

2. Vytvořme secret s connection stringem:

```
kubectl create secret generic app --from-literal=ConnectionStrings__DB="Data Source=tcp:mssql-service.mssql.svc.cluster.local,1433; Initial Catalog=NorthwindTest; User ID=NorthwindTestLogin; Password=AppPass_1"
```

3. Upravte soubor `app-deployment.yml` tak, aby u kontejneru požadoval environment proměnnou `ConnectionStrings__DB` a bral ji ze secret setu s názvem `app` pod klíčem `ConnectionStrings__DB`.

> Jako vzor vezměte definici proměnné `SA_PASSWORD` v souboru `sql-deployment.yml` - udělejte to stejně.

4. Nasaďte upravený soubor do clusteru pomocí `kubectl apply -f app-deployment.yml`.

5. Zkontrolujte, že se po přihlášení načte seznam regionů.
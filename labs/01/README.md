# Lab 01

Cílem tohoto labu je zprovoznit vzorovou webovou aplikaci v kontejneru a spustit ji lokálně.

## Krok 1 - nejjednodušší možný kontejner

1. Stáhněte si tento git repozitář.

2. Otevřete solution `src/NorthwindStore.sln` ve Visual Studiu a ověřte, že jde zkompilovat.

3. Otevřete svůj oblíbený shell.

4. Pomocí `cd` se navigujte do složky `src/NorthwindStore.App`.

5. Spusťte příkaz `dotnet publish -o ../../labs/01/publish -c Release`

6. Podívejte se do složky `labs/01/publish`, zdali se výstup podařil.

7. V shellu se odnavigujte do složky `labs/01`.

8. Prohlédněte si připravený `Dockerfile`.

9. Spusťte příkaz `docker build -f Dockerfile -t northwindstore:0.1 .`

10. Ověřte pomocí `docker image ls`, že se container image vyrobil.

11. Spusťte kontejner pomocí `docker run -it -p 5000:80 --rm northwindstore:0.1`

12. V prohlížeči na adrese http://localhost:5000 ověřte, že aplikace nastartuje.

> _V aplikaci se po spuštění nenačtou CSS styly, takže bude vypadat divně._
>
> _Po přihlášení (user name `admin`, heslo `admin`) aplikace vyhodí chybu 500._
>
> _Obojí je zatím v pořádku, budeme řešit později._

13. Zastavte kontejner pomocí zkratky `Ctrl+C` v shellu.

14. Pomocí příkazu `docker ps` ověřte, že žádný kontejner neběží.

## Krok 2 - diagnostika, proč aplikace vyhazuje 500

Aplikaci jsme zkompilovali v release módu, proto nezobrazuje chybové hlášky. Řešením je spustit ji s environment proměnnou `ASPNETCORE_ENVIRONMENT` nastavenou na hodnotu `Development`.

1. Přidejte do příkazu `docker run` mezi ostatní optiony parametr `-e ASPNETCORE_ENVIRONMENT=Development` a donuťte aplikaci zobrazit chybovou stránku, kde jsou detaily chyby.

> Pozor, option `-e` je třeba předat před název image a tagu. Pokud bychom ji přidali nakonec, pošle se to jako argument do procesu `dotnet` uvnitř kontejneru. 

## Krok 3 - spuštění SQL databáze v kontejneru

1. Odnavigujte se v shellu do složky `labs/01/sql`

2. Prohlédněte si `Dockerfile`

3. Prohlédněte si skripty `entrypoint.sh` a `init-db.sh`, které při startu serveru čekají, než bude dostupný, a poté naseedují databázi.

4. Vytvořte container image pomocí `docker build -f Dockerfile -t mssql-northwind .`

5. Spusťte databázi pomocí příkazu `docker run -d -p:51433:1433 --name northwind-db mssql-northwind`

6. Pomocí příkazu `docker logs northwind-db`

7. Ověřte, že se k databázi lze připojit z hostitelského počítače - hostname serveru je `tcp:localhost,51433` (čárka před číslem portu je správně).

## Krok 4 - připojení aplikace k databázi

Aby se aplikace připojila k databázi, potřebuje přepsat connection string v souboru `appsettings.json`, který je na cestě `ConnectionStrings:DB`. 

To lze udělat přes environment proměnnou, akorát namísto dvojtečky (`:`) se v názvu proměnné použijí dvě podtržítka (`__`).

Aby na sebe kontejnery viděly, je třeba ještě vytvořit Docker network. 

1. Spusťte příkaz `docker network create northwind-network`

2. Připojte SQL Server kontejner do sítě pomocí `docker network connect northwind-network northwind-db`

3. Spusťte aplikaci pomocí `docker run -it -p 5000:80 -e 'ConnectionStrings__DB=Data Source=tcp:northwind-db,1433; Initial Catalog=Northwind; User ID=sa; Password=DevPass_1' --rm --network northwind-network northwindstore:0.1`

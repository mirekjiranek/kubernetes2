# Lab 03

Cílem tohoto labu bude vytvořit konfiguraci pro `docker-compose`, která zajistí, že při startu projektu budeme mít k dispozici jak aplikaci, tak databázi.

## Krok 1 - vygenerování výchozího docker compose skriptu

1. Ve Visual Studiu klikněte pravým tlačítkem na projekt `NorthwindStore.App` a vyberte __Add > Container Orchestrator Support..__.

2. V následujícím okně vyberte možnost __Docker Compose__.

3. Ujistěte se, že jako startup project je vybrán nově vytvořený projekt `docker-compose`.

4. Ověřte, že aktivní build konfigurace je __Release__.

5. Prohlédněte si vygenerované soubory `docker-compose.yml` a `docker-compose.override.yml`.

> Vygenerované soubory počítají jen s jedním kontejnerem s aplikací. Netuší nic o tom, že aplikace potřebuje databázi - konfiguraci pro druhý kontejner tedy musíme přidat.

## Krok 2 - přidání kontejneru s databází

1. Pomocí Docker Dashboardu smažte všechny kontejnery, které máte vytvořeny (databázi a případně kontejnery aplikace).

2. Otevřete soubor `docker-compose.yml` a přidejte do něj druhou service, která bude reprezentovat kontejner s databází. 

    * Pojmenujte ji `northwindstore.db`

    * `Dockerfile` pro ni již máme ve složce `../labs/01/sql`, context musí být složka, kde se Dockerfile nachází.

    * Nezapomeňte přejmenovat vlastnost `image` - např. na `${DOCKER_REGISTRY-}northwindstoredb`

3. Otevřete soubor `docker-compose.override.yml` a přidejte do něj druhou service. Nepotřebuje žádné volumes ani environment proměnné, ale je třeba jí vyvést port.

```
    ports:
      - "51443:1443"
```

4. Aplikačnímu kontejneru v `docker-compose.override.yml` přidejte environment proměnnou `ConnectionStrings__DB` tak, aby viděl na databázi. Connection string je stejný, hostname je název service v docker-compose, tedy `northwindstore.db`.

> Connection string neobsahuje žádné problematické znaky, které by bylo třeba escapovat. Hodnotu connection stringu dejte beze změn přímo za rovnítko.

5. Spusťte aplikaci a ověřte, že funguje.

## Krok 3 - nastavení volume pro databázi

Problém současného stavu je, že jakmile spustíme projekt znovu, databáze se zruší a kontejner se začne vytvářet znovu. 

1. Vytvořte prázdnou složku `c:\temp\sqldata`, kam se uloží soubory databáze.

2. Do souboru `docker-compose.override.yml` přidejte službě `northwindstore.db` konfiguraci pro volume:

```
    volumes:
     - c:/temp/sqldata:/var/opt/mssql/data
```


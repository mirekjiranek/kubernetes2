# Lab 02

Cílem tohoto labu je upravit Dockerfile tak, aby kompilace probíhala deterministicky v kontejneru. 

Díky tomu bude snadné aplikaci zkompilovat v CI prostředí, a navíc nebude vyžadovat instalaci všech potřebných SDK (pro kompilaci je totiž třeba mít správnou verzi .NET SDK a správnou verze Node.js).

## Krok 1 - vytvoření Dockerfile z Visual Studia

1. Ve Visual Studiu klikněte pravým tlačítkem na projekt `NorthwindStore.App` a vyberte možnost __Add > Docker Support...__. 

2. V následujícím okně vyberte prostředí __Linux__.

3. Prohlédněte si výsledný `Dockerfile`.

4. V menu __Debug__ vyberte položku __NorthwindStore.App Debug Properties__ a v poli _Environment Variables_ nastavte proměnnou `ConnectionStrings__DB` na následující hodnotu:

```
Data Source=tcp:northwind-db,1433; Initial Catalog=Northwind; User ID=sa; Password=DevPass_1
```

> Alternativní postup je upravit ručně soubor `launchSettings.json`, který najdete ve složce `Properties`.

5. Do pole __Docker Run Arguments__ přidejte `--network northwind-network`

6. Ujistěte se, že máte aktivní build konfiguraci __Debug__ a zkuste aplikaci spustit pomocí __F5__. Provede se zkrácený běh, takže to netrvá nijak dlouho.

Po opravě by se aplikace měla spustit a měla by být schopna komunikovat s databází.

> Aplikace stále vypadá divně, protože jsme ještě neprovedli kompilaci CSS stylů.

## Krok 1 (alternativní) - vytvoření Dockerfile z VS Code

1. Je potřeba mít nainstalovanou extension do VS Code "C#" https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp a "Docker" https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker

2. Otevřít projekt ve VS Code ve složce `/src`

3. Pomocí Ctrl+Shift+P otevřete command prompt, vyberte příkaz `Docker: Add Docker Files to Workspace`

4. VS Code vás provede "wizardem" pro vybrání vstupních parametrů pro vygenerování Dockerfile. Vyberte postupně:
  - .NET: ASP.NET Core
  - NorthwindStore.App\NorthwindStore.App.csproj
  - Linux
  - 80
  - No

5. VS Code (narozdíl od Visual Studia) nevygeneruje korektně instrukce pro zkopírování všech .csproj souborů (projektové definice). Proto je potřeba za řádek 
`COPY ["NorthwindStore.App/NorthwindStore.App.csproj", "NorthwindStore.App/"]`
přidat ještě následující dva řádky 
```
COPY ["NorthwindStore.DAL/NorthwindStore.DAL.csproj", "NorthwindStore.DAL/"]
COPY ["NorthwindStore.BL/NorthwindStore.BL.csproj", "NorthwindStore.BL/"]
```

6. VS Code kromě Dockerfilu vytvořil ve složce `.vscode` i soubory `settings.json` a `tasks.json`. Prohlédněte si je.

7. Do souboru `.vscode\tasks.json` najděte sekci s labelem `"label": "docker-run: debug"` a v objektu obsahujícím tento label doplňte do sekce `dockerRun` následující kód
```
                "env": {
                    "ConnectionStrings__DB": "Data Source=tcp:northwind-db,1433; Initial Catalog=Northwind; User ID=sa; Password=DevPass_1"
                },
                "network": "northwind-network"
``` 
Jedná se o connection string k databázi a připojení kontejneru do virtuální sítě.

8. To stejné jako v předchozím kroku proveďte i pro sekci s labelem `"label": "docker-run: release"`

## Krok 2 - release build z Visual Studia

1. Přepněte aktivní build konfiguaci na __Release__.

2. Zkuste projekt spustit pomocí __F5__.

> V tomto kroku proces kompilace selže na kroku `dotnet restore`. Vaším úkolem je zjistit proč, a přidat chybějící řádek do `Dockerfile` tak, aby build prošel a kontejner se spustil.

Po opravě by se aplikace měla spustit a měla by být schopna komunikovat s databází.

## Krok 2 (alternativní) - release build z VS Code

1. Do souboru `.vscode\launch.json` na konec pole `configurations` doplňte následující objekt
```
        {
            "name": "Docker .NET Launch release",
            "type": "docker",
            "request": "launch",
            "preLaunchTask": "docker-run: release",
            "netCore": {
                "appProject": "${workspaceFolder}/NorthwindStore.App/NorthwindStore.App.csproj"
            }
        }
```

2. V záložce "Run and Debug" ve VS Code vyberte z rozbalovací nabídky "Docker .NET Launch release"

3. Zkuste projekt spustit pomocí __F5__.

> V tomto kroku proces kompilace selže na kroku `dotnet restore`. Vaším úkolem je zjistit proč, a přidat chybějící řádek do `Dockerfile` tak, aby build prošel a kontejner se spustil.

Po opravě by se aplikace měla spustit a měla by být schopna komunikovat s databází.

## Krok 3 - kompilace CSS stylů

Aplikace `NorthwindStore.App` obsahuje složku `Styles`, v níž je sada SCSS souborů. Ty je třeba v rámci buildu zkompilovat a vytvořit minifikovaný bundle. 

K tomuto účelu se využívá Node.js, resp. nástroj `npm`. V projektu jsou soubory `package.json` a `package-lock.json` (vygenerovaný), které definují používané balíčky, jejich závislosti, a dále skripty, které kompilaci provedou.

V rámci kompilace je třeba nejprve spustit `npm ci` (projde `package.json` a `package-lock.json` a do složky `node_modules` stáhne a připraví všechny potřebné balíčky). 

Následně se spustí `npm run build`, který provede samotnou kompilaci a výstupy uloží do složky `wwwroot`.

1. Otevřete `Dockerfile` vygenerovaný Visual Studiem/VS Codem. (Pokud nemáte možnost vygenerovat Dockerfile pomocí VS nebo VS Code, použijte Dockerfile z `labs/02/Dockerfile`. Je pro něj ale stále potřeba udělat úpravu z Kroku 2, aby fungoval příkaz "dotnet restore".)

2. Za stage `build` přidejte následující sekvenci:

```
FROM node:16 as build-node
WORKDIR "/NorthwindStore.App"
COPY ["NorthwindStore.App/package.json", "."]
COPY ["NorthwindStore.App/package-lock.json", "."]
RUN npm ci
COPY ["/NorthwindStore.App/Styles/", "Styles/"]
RUN npm run build
```

3. Prohlédněte  a rozmyslete si, co sekvence dělá.

4. Dále je třeba do `Dockerfile` přidat poslední řádek, který zkopíruje výsledky kompilace do výstupního image. Vaším úkolem je rozmyslet si, kam tento řádek přesně patří, a přidat jej na ideální místo:

```
COPY --from=build-node /NorthwindStore.App/wwwroot wwwroot/
```

5. Proveďte build docker image z terminálu a následně spusťe kontejner v "--detach" režimu. Nezapomeňte mu předat všechny potřebné parametry pro spuštění (network, porty, connection string), podobným způsobem jako v labu 01.
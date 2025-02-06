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

## Krok 2 - release build

1. Přepněte aktivní build konfiguaci na __Release__.

2. Zkuste projekt spustit pomocí __F5__.

> V tomto kroku proces kompilace selže na kroku `dotnet restore`. Vaším úkolem je zjistit proč, a přidat chybějící řádek do `Dockerfile` tak, aby build prošel a kontejner se spustil.

Po opravě by se aplikace měla spustit a měla by být schopna komunikovat s databází.

## Krok 3 - kompilace CSS stylů

Aplikace `NorthwindStore.App` obsahuje složku `Styles`, v níž je sada SCSS souborů. Ty je třeba v rámci buildu zkompilovat a vytvořit minifikovaný bundle. 

K tomuto účelu se využívá Node.js, resp. nástroj `npm`. V projektu jsou soubory `package.json` a `package-lock.json` (vygenerovaný), které definují používané balíčky, jejich závislosti, a dále skripty, které kompilaci provedou.

V rámci kompilace je třeba nejprve spustit `npm ci` (projde `package.json` a `package-lock.json` a do složky `node_modules` stáhne a připraví všechny potřebné balíčky). 

Následně se spustí `npm run build`, který provede samotnou kompilaci a výstupy uloží do složky `wwwroot`.

1. Otevřete `Dockerfile` vygenerovaný Visual Studiem.

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
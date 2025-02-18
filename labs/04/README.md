# Lab 04

Cílem tohoto labu je založení __Azure Container Registry__ a vypublikování kontejneru.

## Krok 1 - založení ACR

1. Otevřete [Azure Portal](https://portal.azure.com) a přihlaste se.

2. Vyberte vhodnou Azure subscription a založte v ní __Resource group__ (ideální region je __West Europe__).

3. Vyberte možnost __Create a resource__.

4. Vyhledejte resource __Container Registry__.

    * Zvolte právě vytvořenou resource group.

    * Vyberte vhodný název.
    
    * Vyberte SKU __Basic__.

    * Ostatní nastavení nechte na výchozích hodnotách.

5. Potvrďte a počkejte, než se registry vytvoří.

## Krok 2 - autentizace

1. Stáhněte si Azure CLI installer ze stránky https://docs.microsoft.com/en-us/cli/azure/install-azure-cli

2. V shellu spusťte `az login` a v okně prohlížeče, které se otevře, se přihlaste.

3. Spusťte příkaz `az acr login --name <NAZEV_REGISTRY>`.

## Krok 3 - publikování kontejneru

1. V PowerShellu se navigujte do složky `labs/04`.

2. Spusťte Powershell skript `./push-containers.ps1 -Tag 1.0 -RegistryUrl <NAZEV_REGISTRY>.azurecr.io`

3. V portálu v sekci __Repositories__ ověřte, že vypublikovaný kontejner vidíte.

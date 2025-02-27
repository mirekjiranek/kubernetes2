# Lab 14

Cílem tohoto labu je zprovoznění volume pro adresář `wwwroot/images` v aplikačním kontejneru, který chceme mít persistentní.

## Krok 1 - Vytvoření volume claimu

1. Ověřte v aplikaci, že se po přihlášení na záložce __Categories__ načítají obrázky, protože jsou nahrané přímo v kontejneru.

2. Přepněte si kontext do namespace pomocí příkazu `kubectl config set-context --current --namespace=northwind-test`

3. Prozkoumejte soubor `app-volumes.yml` a nasaďte jej do clusteru.

4. Zkontrolujte stav persistent volume claimu pomocí `kubectl get pvc` a případně `kubectl describe pvc/app-files`.

5. Do `app-deployment.yaml` přidejte konfiguraci volume - měl by směřovat na cestu `/app/wwwroot/images`. Postupujte podle návodu v [oficiální dokumentaci](https://docs.microsoft.com/en-us/azure/aks/azure-files-dynamic-pv#use-the-persistent-volume) nebo podle souboru `sql-deployment.yml` v předchozím labu.

6. Nasaďte soubor `app-deployment.yaml`.

## Krok 2 - otestování, že je úložiště persistentní

1. Ověřte, že se nyní obrázky v aplikaci přestanou zobrazovat (pokud se stále zobrazují, zkuste Ctrl+F5, aby se ignorovala cache browseru).

2. V Azure portálu otevřete resource group, která se vytvořila pro váš cluster - bude začínat slovem `MC_` a bude obsahovat název vaší resource group, název AKS clusteru a lokaci. 

3. Najděte storage account a pomocí funkce __Storage browser__ otevřete příslušné Azure files pro náš persistent volume claim.

4. Vytvořte složku `categories`.

5. Nahrajte do ní správné obrázky.

6. V aplikaci u některé z kategorií změňte její obrázek.

7. Ověřte v Azure portálu, že se obrázek skutečně uložil.

8. Zabijte pod aplikace pomocí `kubectl delete pod <POD_NAME>`. Cluster by si hned měl vytvořit nový.

9. Ověřte, že se nahraný obrázek zachoval.  


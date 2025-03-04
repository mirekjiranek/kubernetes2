# Lab 20

Cílem tohoto labu je opětovné vytvoření Kubernetes clusteru a opětovné nasazení aplikace - tentokrát ji nasadíme dvakrát - jednou v testovací verzi a jednou v produkční.

## Krok 1 - Vytvoření ACR a AKS

1. Spusťte PowerShell a přihlaste se do Azure CLI pomocí `az login`

2. Přepněte se do správné subscription `az account set --subscription <SUBSCRIPTION_NAME_OR_ID>`

3. Uložte si do proměnné název resource group, do které budeme prostředky vytvářet: `$resourceGroup = "<RESOURCE_GROUP_NAME>"`

4. Zadejte název Azure Container Registry z minula, nebo si vytvořte nový (musí to být DNS název, takže jen malá písmena, čísla a pomlčky): `$registryName = "<YOUR_CHOSEN_REGISTRY_NAME>"`

> Pokud jste zadali jiný název Azure Container Registry, budete muset upravit deployment YAML soubory z minula.

5. Zadejte název clusteru z minula (pokud si pamatujete), nebo si vymyslete nový: `$clusterName = "<YOUR_CHOSEN_CLUSTER_NAME>"`

6. Nyní vytvořte Azure Container Registry: 

```
az group create --name $resourceGroup --location westeurope
az acr create --name $registryName --location westeurope --resource-group $resourceGroup --sku Basic
```

7. Následně vytvořte AKS cluster:

```
az aks create --resource-group $resourceGroup --name $clusterName --node-count 1 --node-vm-size Standard_D2s_v4 --node-osdisk-size 32 --generate-ssh-keys --location westeurope --kubernetes-version 1.32.0 --attach-acr $registryName
```

8. Autentizujte si svůj Docker k vytvořenému Azure Container Registry:

```
az acr login --name $registryName
```

9. Autentizujte si svůj `kubectl` k vytvořenému Azure Kubernetes Service:

```
az aks get-credentials --resource-group $resourceGroup --name $clusterName --overwrite-existing
```

## Krok 2 - Publish kontejneru

1. V PowerShellu se navigujte do složky `labs/04`.

2. Spusťte Powershell skript `./push-containers.ps1 -Tag 1.0 -RegistryUrl "$registryName.azurecr.io"`

> Pokud Docker hlásí, že nemůže najít image k publishování, otevřete solution ve Visual Studiu, nastavte jako startup project __NorthwindStore.App__, přepněte se do konfigurace __Release__ a spusťte build. To by mělo kontejner vytvořit.

3. V Azure portálu najděte váš Azure Container Registry a v sekci __Repositories__ ověřte, že vypublikovaný kontejner vidíte.

## Krok 3 - Deployment databáze pro test a produkci

1. Přejděte v shellu na cestu `labs/13`

2. Vytvořte namespace pro SQL Server a nasaďte volumes a aplikaci:

```
kubectl create namespace mssql
kubectl config set-context --current --namespace=mssql
kubectl apply -f sql-volumes.yml
kubectl create secret generic mssql --from-literal=SA_PASSWORD="DevPass_1"
kubectl apply -f sql-deployment.yml
```

3. Pomocí `kubectl port-forward` se k serveru připojte (port 1433 na vašem počítači může být blokován, namapujte si jej tedy třeba takto - `51433:1433`).

```
kubectl port-forward service/mssql-service 51433:1433
```

4. V SQL Server Management Studiu se připojte k serveru `tcp:127.0.0.1,51433`, uživatel `sa`, heslo `DevPass_1`

5. Spusťte proti serveru skript `labs/20/northwindtest_with_users.sql`. Může to chvíli trvat.

6. Spusťte proti serveru skript `labs/20/northwindprod_with_users.sql`. Může to chvíli trvat.

7. Zabijte port forwarding tunel pomocí Ctrl+C.

## Krok 4 - Nasazení aplikace

1. Vytvořte namespace pro testovací verzi aplikace a připravme secret s connection stringem:

```
kubectl create namespace northwindstore-test
kubectl create secret generic app --from-literal=ConnectionStrings__DB="Data Source=tcp:mssql-service.mssql.svc.cluster.local,1433; Initial Catalog=NorthwindTest; User ID=NorthwindTestLogin; Password=AppPass_1" --namespace=northwindstore-test
```

2. Vytvořte namespace pro produkční verzi aplikace a připravme secret s connection stringem:

```
kubectl create namespace northwindstore-prod
kubectl create secret generic app --from-literal=ConnectionStrings__DB="Data Source=tcp:mssql-service.mssql.svc.cluster.local,1433; Initial Catalog=NorthwindProd; User ID=NorthwindProdLogin; Password=9f&@Jj@6uCRHpDpn" --namespace=northwindstore-prod
```

3. Ujistěte se, že jste v adresáři `labs/14`

4. Zkontrolujte, že v něm máte poslední verzi souboru `app-deployment.yml`:

    * Měl by v ní být odkaz na volume.

    * Měla by v ní být environment proměnná `ConnectionStrings__DB`, která se odkazuje do secrets.

4. Nasaďte volumes a aplikaci pro testovací a produkční verzi aplikace:

```
kubectl apply -f app-volumes.yml --namespace northwindstore-test
kubectl apply -f app-deployment.yml --namespace northwindstore-test
kubectl apply -f app-volumes.yml --namespace northwindstore-prod
kubectl apply -f app-deployment.yml --namespace northwindstore-prod
```

5. Nakopírujte obrázky do volume aplikace:

```
$podName = kubectl get pods --namespace northwindstore-test -o custom-columns=":metadata.name" --no-headers
kubectl cp ../../src/NorthwindStore.App/wwwroot/images/categories "${podName}:/app/wwwroot/images" --namespace northwindstore-test

$podName = kubectl get pods --namespace northwindstore-prod -o custom-columns=":metadata.name" --no-headers
kubectl cp ../../src/NorthwindStore.App/wwwroot/images/categories "${podName}:/app/wwwroot/images" --namespace northwindstore-prod
```

6. Přejděte do adresáře `labs/20`

7. Nasaďte service pro testovací a produkční verzi:

```
kubectl apply -f app-service-test.yml --namespace northwindstore-test
kubectl apply -f app-service-prod.yml --namespace northwindstore-prod
```

8. Pomocí `kubectl get services --all-namespaces` vypište veřejné IP adresy a zkontrolujte, že testovací i produkční verze aplikace běží:

    * http://<IP_ADDRESS_TEST>:5000/
    * http://<IP_ADDRESS_PROD>:5001/
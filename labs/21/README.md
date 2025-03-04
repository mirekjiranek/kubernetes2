# Lab 21

Cílem tohoto labu je instalace ingress controlleru do clusteru a úprava služeb, aby nebyly vystaveny ven přes vnější load balancer.

## Krok 1 - Instalace ingress controlleru

1. Pomocí příkazu `helm version` zjistěte, jestli máte nainstalovaný Helm (package manager pro Kubernetes, budeme si o něm povídat později).

2. Pokud ne, stáhněte jej z [Releases stránky](https://github.com/helm/helm/releases) a rozbalte jej do adresáře, který máte v proměnné PATH.

> Pokud máte Chocolatey, můžete místo toho spustit `choco install kubernetes-helm`

3. Spusťte následující příkazy:

```
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm install ingress-nginx ingress-nginx/ingress-nginx `
  --create-namespace `
  --namespace ingress-default `
  --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-load-balancer-health-probe-request-path"=/healthz
```

4. Pomocí příkazu `kubectl --namespace ingress-default get services` sledujte, kdy dojde k přidělení externí IP adresy.

## Krok 2 - Úprava služby, aby nebyla publikována na load balancer

Testovací verzi aplikace budeme chtít rozběhnout na adrese `test.northwindstore.local`, produkci pak na `www.northwindstore.local`. 

1. Nejprve vytvořte soubor `app-service.yml`, který budeme používat pro test i pro produkci. 

    * Použijte soubor z labu `labs/20` (je jedno jestli testovací nebo produkční).
    
    * Vnější port služby změňte na 80.

    * Odeberte `type: LoadBalancer` - službu nechceme publikovat ven.

2. Nasaďte službu jak pro test, tak pro produkci:

```
kubectl apply -f app-service.yml --namespace northwindstore-test
kubectl apply -f app-service.yml --namespace northwindstore-prod
```

## Krok 3 - Příprava YAML pro Ingress

1. Upravte soubor `app-ingress-test.yml` tak, aby:

    * Měl název `app-ingress`

    * Měl stejné labely jako aplikace a další resources.

    * Směřoval na doménu `test.northwindstore.local`

    * Odkazoval na správný název služby.

2. Obdobně vytvořte `app-ingress-prod.yml`:

    * Měl by směřovat na doménu `www.northwindstore.local`

3. Nasaďte ingressy do clusteru:

```
kubectl apply -f app-ingress-test.yml --namespace northwindstore-test
kubectl apply -f app-ingress-prod.yml --namespace northwindstore-prod
```

4. Pomocí `kubectl --namespace ingress-default get services` zjistěte vnější IP adresu ingress controlleru.

5. Otevřete (s admin právy) soubor `C:\Windows\System32\drivers\etc\hosts` a přidejte do něj následující řádky:

```
<IP_ADDRESS> www.northwindstore.local
<IP_ADDRESS> test.northwindstore.local
<IP_ADDRESS> northwindstore.local
```

6. Otestujte, že aplikace fungují:

    * http://test.northwindstore.local
    * http://www.northwindstore.local

## Krok 4 - Redirect na `www.*`

Pokud uživatel zadá adresu bez `www.`, tedy http://northwindstore.local, do produkční verze aplikace se nedostane.

1. Zkopírujte soubor `app-ingress-prod.yml` a pojmenujte jej `app-ingress-prod-redirect.yml`.

2. Proveďte tyto změny:

    * Přejmenujte jej na `app-ingress-redirect`

    * Pod element `metadata` přidejte tuto sekci:

```
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: http://www.northwindstore.local/$1
```

    * Upravte hostname na `northwindstore.local`

    * Vlastnost `path` upravte na `/(.*)`, aby se do capture group `$1` namapovala URL v rámci aplikace.

3. Nasaďte soubor do clusteru pomocí `kubectl apply -f app-ingress-prod-redirect.yml --namespace northwindstore-prod`

4. Ověřte, že po otevření stránky http://northwindstore.local budete přesměrováni na verzi s `www`.

5. Přihlaste se do aplikace a ověřte, že i http://northwindstore.local/admin/RegionList redirectuje správně.
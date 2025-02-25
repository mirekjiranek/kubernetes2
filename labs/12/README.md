# Lab 12

Cílem tohoto labu je ruční nasazení aplikace do clusteru.

## Krok 1 - Příprava a nasazení deploymentu

1. Otevřete soubor `labs/12/app-deployment.yml` - je vykopírovaný z [oficiální dokumentace](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/).

2. Upravte jej tak, aby:

    * Název deploymentu byl `app-deployment`

    * Labely chceme `app: northwindstore` a `component: app`

    * Chceme zatím 1 repliku

    * Název kontejneru bude `northwindstore-app`

    * Container image je `<REGISTRY_URL>/northwindstore/app:1.0`

3. Založte v clusteru namespace `northwind-test` pomocí příkazu `kubectl create namespace northwind-test`

4. Přepněte si kontext do tohoto namespace pomocí příkazu `kubectl config set-context --current --namespace=northwind-test`

5. Ujistěte se, že jste v shellu na cestě `labs/12`

6. Nasaďte deployment pomocí `kubectl apply -f app-deployment.yml`

7. Zkontrolujte stav deploymentu pomocí příkazu `kubectl describe deployment/app-deployment`

8. Podívejte se na pody, které v našem namespace běží `kubectl get pods`

9. Pomocí `kubectl logs <POD_NAME>`

10. Ověřte, že aplikace naběhne na stránce http://localhost:5000, pokud si do ní vytvoříme tunel:

```
kubectl port-forward pods/<POD_NAME> 5000:80
```

> Databázi jsme nekonfigurovali, takže ta zatím fungovat nebude.

11. Zmáčkněte Ctrl+C, aby se příkaz ukončil.

## Krok 2 - Příprava a nasazení service

1. Otevřete soubor `labs/12/app-service.yml` - je vykopírovaný z [oficiální dokumentace](https://kubernetes.io/docs/concepts/services-networking/service/).

2. Upravte jej tak, aby:

    * Název služby byl `app-service`

    * Labely chceme stejné jako u deploymentu, tj. `app: northwindstore` a `component: app`

    * Vnější port (parametr `port`) bude `5000`

    * Vnitřní port (to, na kterém běží aplikace v kontejnerech - `targetPort`) je `80`.
    
    * Na konec souboru přidejte typ služby `  type: LoadBalancer`. Pozor na odsazení - je to vlastnost elementu `spec`, ne portu.

3. Ujistěte se, že jste v shellu na cestě `labs/12`

4. Nasaďte deployment pomocí `kubectl apply -f app-service.yml`

5. Zkontrolujte stav služby pomocí `kubectl get service`

6. Pomocí `kubectl describe service/app-service` zjištěte IP adresu, na které služba běží, a ověřte v prohlížeči, že funguje (běží na portu 5000).

> Databázi jsme nekonfigurovali, takže ta zatím fungovat nebude.


# Lab 11

Cílem tohoto labu je založit __Azure Kubernetes Service__ cluster.

## Krok 1 - založení AKS

1. Otevřete shell a spusťte příkaz `az login`, abyste se přihlásili k Azure API.

2. Najděte si přesný název Azure subscription a spusťte příkaz `az account set --subscription "<SUBSCRIPTION_NAME>"`

3. Najděte si přesný název Azure resource group, ve které je Azure Container Registry, a spusťte tento příkaz:

```
az aks create --resource-group <RESOURCE_GROUP> --name <CLUSTER_NAME> --node-count 1 --node-vm-size Standard_D2s_v4 --node-osdisk-size 32 --generate-ssh-keys --location westeurope --kubernetes-version 1.32.0
```

4. Přihlaste `kubectl` do clusteru pomocí tohoto příkazu:

```
az aks get-credentials --resource-group <RESOURCE_GROUP> --name <CLUSTER_NAME>
```

5. Ověřte, že vidíte na cluster pomocí příkazu `kubectl get nodes`

## Krok 2 - propojení Azure Container Registry s AKS

Aby mohl Kubernetes cluster stahovat kontejnery z Azure Container Registry, je třeba mezi těmito službami nastavit autentizaci. Jde to kompletně bez hesel nebo secretů, pokud máte práva na obě služby.

1. Spusťte příkaz `az aks update --resource-group <RESOURCE_GROUP> --name <CLUSTER_NAME> --attach-acr <REGISTRY_NAME>`

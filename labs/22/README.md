# Lab 22

Cílem tohoto labu je vygenerovat self-signed certifikáty a nasadit je na ingress.

## Krok 1 - Vygenerování certifikátu pro testovací prostředí

1. Ujistěte se, že máte nainstalované OpenSSL, pomocí příkazu `openssl version`.

2. Pokud ne, nainstalujte například odsud: https://slproweb.com/products/Win32OpenSSL.html 

3. Vygenerujte self-signed certifikát:

```
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -out aks-ingress-tls.crt -keyout aks-ingress-tls.key -subj "/CN=test.northwindstore.local/O=Northwind Store"
```

4. Vytvořte secret pro certifikát: 

```
kubectl create secret tls ingress-tls --namespace northwindstore-test --key aks-ingress-tls.key --cert aks-ingress-tls.crt
```

5. Vezměte YAML soubor `app-ingress-test.yml` a doplňte do sekce `spec` následující parametry:

```
  tls:
  - hosts:
    - test.northwindstore.local
    secretName: ingress-tls
```

6. Nasaďte změnu do clusteru pomocí `kubectl apply -f .\app-ingress-test.yml --namespace northwindstore-test`

7. Ověřte, že nastavení zabralo: https://test.northwindstore.local

> Web bude stále nedůvěryhodný, ale když zobrazíte certifikát, uvidíte, že organizace je __Northwind store__ a ne __Kubernetes Ingress Controller Fake Certificate__.

## Krok 2 - Vygenerování certifikátu pro produkci

1. Vygenerujte self-signed certifikát s hlavní doménou `northwindstore.local` a Subject Alternative Name `www.northwindstore.local`:

```
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -out aks-ingress-tls-prod.crt -keyout aks-ingress-tls-prod.key -subj "/CN=northwindstore.local/O=Northwind Store" -addext "subjectAltName = DNS:www.northwindstore.local"
```

2. Vytvořte secret pro certifikát: 

```
kubectl create secret tls ingress-tls --namespace northwindstore-prod --key aks-ingress-tls-prod.key --cert aks-ingress-tls-prod.crt
```

3. Vezměte YAML soubory `app-ingress-prod.yml` a `app-ingress-prod-redirect.yml` a doplňte do sekce `spec` následující parametry:

```
  tls:
  - hosts:
    - northwindstore.local
    secretName: ingress-tls
```

4. Nasaďte ingressy do clusteru:

```
kubectl apply -f app-ingress-prod.yml -f app-ingress-prod-redirect.yml --namespace northwindstore-prod
```

5. Ověřte, že se používá správný certifikát:

    * https://www.northwindstore.local
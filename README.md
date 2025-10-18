# Microservice Eğitimi 1 - Bilginç IT Academy

## Ders Programı
- [ ] Transaction (Saga Pattern)

## 18.10.2025
- [x] Ocelot ile Gateway(son bir özetledik ve C# configuration kullanmaya çalıştık)
- [x] YARP ile Gateway
- [x] YARP LoadBalance
- [x] YARP Ratelimit
- [x] YARP Authentication/Authorization
- [x] YARP HealthCheck
- [x] Ocelot vs YARP
- [x] Observability (with OpenTelemetry and Jeager)
- [x] Aspire
- [x] Microservice Patterns
    - [x] Shared Database Anti pattern (Teorik)
    - [x] Database-per Service Pattern (Teorik)
    - [x] API Composition Pattern

## 17.10.2025
- [x] HashiCorp.Vault
- [x] QoS / Retry / Circuit Breaker(Tam çalıştıramadık, Ocelot)
- [x] Docker image oluşturma
- [x] Docker compose
- [x] LoadBalance (Ocelot)
- [x] RateLimit (Ocelot)
- [x] Service Discovery with Ocelot(Tam çalıştıramadık, Consul)
- [x] Authentication
- [x] Authorization

## 16.10.2025
- [x] Architectural patterns
- [x] Microservice nedir?
- [x] Modular Monolith nedir?
- [x] Microservice vs Modular Monolith
- [x] WebAPI ile Microservice inşa edelim
- [x] OpenApi and Scalar
- [x] Health Check
- [x] Service Discovery Pattern (HashiCorp Consul)
- [x] Resilience Pattern (Polly)
- [x] Gateway nedir?
- [x] Ocelot ile Gateway (Kurulum, Projeleri ekleme)
- [x] CORS politikası


## Consul Docker komutu (Service Discovery)
```powershell
docker run -d --name consul -p 8500:8500 hashicorp/consul:latest
```

## Polly kütüphanesi BackoffType
```csharp
//🧩 DelayBackoffType Enum Türleri
//Constant	Her denemede sabit süre bekler.	Delay = 5s → 5s, 5s, 5s
//Linear	Her denemede gecikme lineer (doğrusal) artar.	Delay = 5s → 5s, 10s, 15s
//Exponential	Her denemede gecikme katlanarak (üstel) artar.	Delay = 5s → 5s, 10s, 20s, 40s
```

## HasiCorp Vault 
- Development Docker
```powershell
docker run -d --name vault -p 8200:8200 --cap-add=IPC_LOCK -e VAULT_DEV_ROOT_TOKEN_ID=root -e VAULT_ADDR=http://0.0.0.0:8200 hashicorp/vault:latest server -dev
```

- NuGet Package
```dash
VaultSharp
```

- C# kodları
```csharp
public class VaultService
{
    public async Task<Secret<SecretData>> GetSecrets()
    {
        var vaultToken = "root";
        var vaultUri = "http://127.0.0.1:8200";
        var vaultTokenInfo = new TokenAuthMethodInfo(vaultToken);
        var vaultClientSettings = new VaultClientSettings(vaultUri, vaultTokenInfo);
        var vaultClient = new VaultClient(vaultClientSettings);

        var secrets = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: "productapp/config",
            mountPoint: "secret");

        return secrets;
    }
}
```

- vault.hcl
```hcl
ui = true

# Bu iki satır eklendi 👇
api_addr    = "http://127.0.0.1:8200"
cluster_addr = "http://127.0.0.1:8201"

storage "raft" {
  path    = "/vault/data"
  node_id = "vault-1"
}

listener "tcp" {
  address          = "0.0.0.0:8200"
  tls_disable      = 1           # test için; prod'da kaldır
  cluster_address  = "0.0.0.0:8201"  # opsiyonel ama eklemek iyi
}

disable_mlock = true
```

- Production Docker (Bu kod vault.hcl in bulunduğu klasörde çalıştırılmalı)
```powershell
docker run -d --name vault -p 8200:8200 --cap-add=IPC_LOCK -v "${PWD}\vault-data:/vault/data" -v "${PWD}\vault.hcl:/vault/config/vault.hcl" hashicorp/vault server -config=vault.hcl
```

## Docker CLI komutları
- Network komutları
```powershell
#docker network listele
docker network ls 

#kullanılmayan networkleri sil
docker network prune 

#yeni network oluştur
docker network create network_name
```

- Image ve container komutları
```powershell
#image dönüştürme - eğer docker file olan ana dizinde ise build komutu
docker build -t image_name . 

#image dönüştürme - eğer docker file alt dizinde ise
docker build -t image_name -f Microservice.ProductWebAPI/Dockerfile . 

#container oluşturma
docker run -d --name container_name -p 6001:8080 image_adi

#networke bağlı container oluşturma
docker run -d --network eticaret --name product -p 6001:8080 productapi 
```

- docker compose build
```powershell
#eğer ilk oluşturuyorsak
docker compose up -d

#eğer tekrar rebuild yapacaksak
docker compose up -d --build
```
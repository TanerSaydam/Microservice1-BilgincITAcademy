# Microservice Eğitimi 1 - Bilginç IT Academy

## Ders Programı
- [ ] HashiCorp.Vault
- [ ] Ocelot ile Gateway
- [ ] QoS / Retry / Circuit Breaker
- [ ] Docker
- [ ] Authentication
- [ ] Authorization
- [ ] LoadBalance
- [ ] RateLimit
- [ ] YARP ile Gateway
- [ ] Ocelot vs YARP
- [ ] Transaction (Saga Pattern)
- [ ] Observability
- [ ] Aspire

## 16.10.2025 1.Ders
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
//🧩 DelayBackoffType Enum Türleri
//Constant	Her denemede sabit süre bekler.	Delay = 5s → 5s, 5s, 5s
//Linear	Her denemede gecikme lineer (doğrusal) artar.	Delay = 5s → 5s, 10s, 15s
//Exponential	Her denemede gecikme katlanarak (üstel) artar.	Delay = 5s → 5s, 10s, 20s, 40s

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
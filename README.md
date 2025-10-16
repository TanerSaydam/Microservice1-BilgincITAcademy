# Microservice Eğitimi 1 - Bilginç IT Academy

## Ders Programı
- [ ] Gateway nedir?
- [ ] Ocelot ile Gateway
- [ ] Docker
- [ ] CORS politikası
- [ ] Authentication
- [ ] Authorization
- [ ] QoS / Retry / Circuit Breaker
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


## Consul Docker komutu (Service Discovery)
```powershell
docker run -d --name consul -p 8500:8500 hashicorp/consul:latest
```

## Polly kütüphanesi BackoffType
//🧩 DelayBackoffType Enum Türleri
//Constant	Her denemede sabit süre bekler.	Delay = 5s → 5s, 5s, 5s
//Linear	Her denemede gecikme lineer (doğrusal) artar.	Delay = 5s → 5s, 10s, 15s
//Exponential	Her denemede gecikme katlanarak (üstel) artar.	Delay = 5s → 5s, 10s, 20s, 40s
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
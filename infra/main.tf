locals { suffix = substr(md5(data.azurerm_client_config.current.subscription_id), 0, 6) }
data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "main" {
  name     = "rg-${var.app_name}-${local.suffix}"
  location = var.location
}

resource "azurerm_container_registry" "main" {
  name                = replace("acr${var.app_name}${local.suffix}", "-", "")
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"
  admin_enabled       = true
}

resource "azurerm_log_analytics_workspace" "main" {
  name                = "log-${var.app_name}-${local.suffix}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "main" {
  name                       = "cae-${var.app_name}-${local.suffix}"
  resource_group_name        = azurerm_resource_group.main.name
  location                   = azurerm_resource_group.main.location
  log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
}

resource "azurerm_postgresql_flexible_server" "main" {
  name                          = "psql-${var.app_name}-${local.suffix}"
  resource_group_name           = azurerm_resource_group.main.name
  location                      = azurerm_resource_group.main.location
  version                       = "16"
  administrator_login           = var.db_admin_username
  administrator_password        = var.db_admin_password
  sku_name                      = "B_Standard_B1ms"
  storage_mb                    = 32768
  public_network_access_enabled = true
  backup_retention_days         = 7
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "azure" {
  name             = "AllowAzureServices"
  server_id        = azurerm_postgresql_flexible_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_postgresql_flexible_server_database" "main" {
  name      = "flightsdb"
  server_id = azurerm_postgresql_flexible_server.main.id
  collation = "en_US.utf8"
  charset   = "UTF8"
}

resource "azurerm_container_app" "main" {
  name                         = "ca-${var.app_name}-${local.suffix}"
  container_app_environment_id = azurerm_container_app_environment.main.id
  resource_group_name          = azurerm_resource_group.main.name
  revision_mode                = "Single"

  secret {
    name  = "db-connection"
    value = "Host=${azurerm_postgresql_flexible_server.main.fqdn};Port=5432;Database=${azurerm_postgresql_flexible_server_database.main.name};Username=${var.db_admin_username};Password=${var.db_admin_password};SSL Mode=Require;Trust Server Certificate=true"
  }
  secret { name = "acr-password" value = azurerm_container_registry.main.admin_password }

  registry {
    server               = azurerm_container_registry.main.login_server
    username             = azurerm_container_registry.main.admin_username
    password_secret_name = "acr-password"
  }

  template {
    min_replicas = 0
    max_replicas = 2
    container {
      name   = "flight-app"
      image  = "mcr.microsoft.com/dotnet/samples:aspnetapp"
      cpu    = 0.5
      memory = "1Gi"
      env { name = "ConnectionStrings__DefaultConnection" secret_name = "db-connection" }
      env { name = "ASPNETCORE_ENVIRONMENT" value = "Production" }
    }
  }

  ingress {
    external_enabled = true
    target_port      = 8080
    traffic_weight { percentage = 100 latest_revision = true }
  }
}

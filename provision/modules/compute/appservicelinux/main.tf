resource "azurerm_linux_web_app" "app_service_linux" {
  name                    = var.app_service_name
  resource_group_name     = var.resource_group_name
  location                = var.location
  service_plan_id         = var.service_plan_id
  https_only              = true
  client_affinity_enabled = var.client_affinity_enabled
  
  site_config {    
    http2_enabled       = true
    always_on           = var.always_on
    minimum_tls_version = var.minimum_tls_version

    health_check_path                 = "/health"
    health_check_eviction_time_in_min = 5
    websockets_enabled                = var.websockets_enabled
    ftps_state                        = "FtpsOnly"
    use_32_bit_worker                 = var.use_32_bit_worker

    cors {
      allowed_origins = var.cors_allowed_origins
    }
    
  }

  app_settings = var.app_settings

  identity {
    type = "SystemAssigned"
  }

  logs {
    http_logs {
      file_system {
        retention_in_days = var.log_file_system.retention_in_days
        retention_in_mb   = var.log_file_system.retention_in_mb
      }
    }
  }

  tags = merge(var.tags, var.policy_tags)

  lifecycle {
    ignore_changes = [tags]
  }
}

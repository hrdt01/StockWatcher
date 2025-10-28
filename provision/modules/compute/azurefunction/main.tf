resource "azurerm_linux_function_app" "azure_function" {
  name                       = var.azure_function_name
  location                   = var.location
  resource_group_name        = var.resource_group_name
  service_plan_id            = var.service_plan_id
  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_accesskey

  site_config {
    application_insights_connection_string = var.appsetting_appinsights_connstr
    
    application_stack {
        dotnet_version              = "8.0"
        use_dotnet_isolated_runtime = true
    }

    cors {
      allowed_origins = var.cors_allowed_origins
    }
  }

  identity {
    type = "SystemAssigned"    
  }

  app_settings = {
    FUNCTIONS_WORKER_RUNTIME               = "dotnet-isolated"
    SCM_DO_BUILD_DURING_DEPLOYMENT         = "false"
    DOTNET_ENVIRONMENT                     = "Production"
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED = 1
    DOTNET_CONTENTROOT                     = "/home/site/wwwroot"

    "StockTracker__Repository__AzureTable__ConnectionString"   = var.appsetting_storage_connectionstring
    "StockTracker__ExtractorFunction__CronExpression"          = var.appsetting_function_cronexpression
    "StockTracker__Repository__AzureQueue__ConnectionString"   = var.appsetting_queue_storage_connectionstring
    "StockTracker__ExtractorFunction__CleanupDeprecatedInfo"   = var.appsetting_function_cleanupprocess_enabled
  }
  
  tags = merge(var.tags, var.policy_tags)
  
  lifecycle {
    ignore_changes = [
      app_settings["OpenApi__ApiKey"],
      app_settings["WEBSITE_ENABLE_SYNC_UPDATE_SITE"],
      app_settings["WEBSITE_RUN_FROM_PACKAGE"],
      tags["hidden-link: /app-insights-instrumentation-key"],
      tags["hidden-link: /app-insights-resource-id"],
      tags["hidden-link: /app-insights-conn-string"]
    ]
  }
}

locals {  

}

# Retrieving resources
data "azurerm_resource_group" "general"{
  name = var.stocktracker_resource_group_name
}

data "azurerm_storage_account" "sa" {
  name = var.stocktracker_storage_account_name
  resource_group_name = data.azurerm_resource_group.general.name
}

data "azurerm_application_insights" "app_insights"{
  name                = var.app_insights_name
  resource_group_name = var.app_insights_rg_name
}

# Provision resources
module "serviceplan" {
  source = "../modules/compute/appserviceplan"

  app_serviceplan_name = var.stocktracker_app_serviceplan_name
  location             = data.azurerm_resource_group.general.location
  resource_group_name  = data.azurerm_resource_group.general.name
  os_type              = "Linux"
  sku_name             = var.stocktracker_app_serviceplan_sku_name
  tags                 = var.terraform_tags
  policy_tags          = var.policy_tags
}

module "functionapp" {
  source = "../modules/compute/azurefunction"
  depends_on = [ module.serviceplan ]
  azure_function_name                        = var.stocktracker_function_name
  location                                   = data.azurerm_resource_group.general.location
  resource_group_name                        = data.azurerm_resource_group.general.name
  service_plan_id                            = module.serviceplan.id
  storage_account_name                       = data.azurerm_storage_account.sa.name
  storage_account_accesskey                  = data.azurerm_storage_account.sa.primary_access_key 
  policy_tags                                = var.policy_tags
  tags                                       = var.terraform_tags
  appsetting_storage_connectionstring        = data.azurerm_storage_account.sa.primary_connection_string
  appsetting_function_cronexpression         = var.stocktracker_function_cronexpression
  appsetting_queue_storage_connectionstring  = data.azurerm_storage_account.sa.primary_connection_string
  appsetting_function_cleanupprocess_enabled = "true"
  appsetting_appinsights_connstr             = data.azurerm_application_insights.app_insights.connection_string
  cors_allowed_origins                       = [ trimsuffix(data.azurerm_storage_account.sa.primary_web_endpoint, "/") ]
}

resource "azurerm_role_assignment" "function_role_over_storage" {
  depends_on = [ module.functionapp ]
  scope                = data.azurerm_storage_account.sa.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = module.functionapp.id
}

resource "azurerm_role_assignment" "function_role_over_queue_storage" {
  depends_on = [ module.functionapp ]
  scope                = data.azurerm_storage_account.sa.id
  role_definition_name = "Storage Queue Data Contributor"
  principal_id         = module.functionapp.id
}
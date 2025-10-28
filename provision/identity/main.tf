locals {
  
}

# Retrieving resources
data "azurerm_resource_group" "general"{
  name = var.stocktracker_resource_group_name
}

data "azurerm_storage_account" "sa" {
  name                = var.stocktracker_storage_account_name
  resource_group_name = data.azurerm_resource_group.general.name
}

data "azurerm_application_insights" "app_insights"{
  name                = var.app_insights_name
  resource_group_name = var.app_insights_rg_name
}

# Provision resources
module "serviceplan" {
  source = "../modules/compute/appserviceplan"

  app_serviceplan_name = var.stocktrackeridentity_app_serviceplan_name
  location             = data.azurerm_resource_group.general.location
  resource_group_name  = data.azurerm_resource_group.general.name
  os_type              = "Linux"
  sku_name             = var.stocktrackeridentity_app_serviceplan_sku_name
  tags                 = var.terraform_tags
  policy_tags          = var.policy_tags
}

module "identityapi" {
  source = "../modules/compute/appservicelinux"
  depends_on = [ module.serviceplan ]
   
  app_service_name                          = var.stocktrackeridentity_appservice_name
  location                                  = data.azurerm_resource_group.general.location
  resource_group_name                       = data.azurerm_resource_group.general.name
  service_plan_id                           = module.serviceplan.id
  policy_tags                               = var.policy_tags
  tags                                      = var.terraform_tags
  
  app_settings                              = { "APPLICATIONINSIGHTS_CONNECTION_STRING" = data.azurerm_application_insights.app_insights.connection_string }
  cors_allowed_origins                      = [ trimsuffix(data.azurerm_storage_account.sa.primary_web_endpoint, "/") ]
}

resource "azurerm_application_insights" "app_insights" {
  name                       = var.app_insights_name
  resource_group_name        = var.resource_group_name
  location                   = var.location
  application_type           = var.application_type
  internet_ingestion_enabled = var.internet_ingestion_enabled
  internet_query_enabled     = var.internet_query_enabled
  daily_data_cap_in_gb       = 1
   
  tags                       = merge(var.policy_tags, var.tags)
}

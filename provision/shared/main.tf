locals {
  
}

data "azurerm_resource_group" "general"{
  name = var.stocktracker_resource_group_name
}

data "azurerm_storage_account" "sa" {
  name                = var.stocktracker_storage_account_name
  resource_group_name = data.azurerm_resource_group.general.name
}

# Provision resources
resource "azurerm_storage_table" "endofdaytable" {
  storage_account_name = data.azurerm_storage_account.sa.name
  name                 = var.stocktracker_endofday_tablename
}

resource "azurerm_storage_queue" "kpimessagebroker" {
  storage_account_name = data.azurerm_storage_account.sa.name
  name                 = var.stocktracker_kpimessagebroker_queuename
}

resource "azurerm_storage_queue" "cleanupmessagebroker" {
  storage_account_name = data.azurerm_storage_account.sa.name
  name                 = var.stocktracker_cleanupmessagebroker_queuename
}
resource "azurerm_storage_account_static_website" "frontend_site" {
  storage_account_id = data.azurerm_storage_account.sa.id
  index_document     = "index.html"
}

resource "azurerm_resource_group" "monitor_rg"{
  name     = var.app_insights_rg_name
  location = data.azurerm_resource_group.general.location
}

module "monitor" {
  depends_on = [ azurerm_resource_group.monitor_rg ]
  source     = "../modules/monitoring/appinsights"

  app_insights_name          = var.app_insights_name
  internet_ingestion_enabled = true
  internet_query_enabled     = true
  location                   = azurerm_resource_group.monitor_rg.location
  resource_group_name        = azurerm_resource_group.monitor_rg.name
  
  tags = {
    Scope           = "MONITORING",
    ApplicationName = "HACTEMA"
  }
  policy_tags = var.policy_tags
}

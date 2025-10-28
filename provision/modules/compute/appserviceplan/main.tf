resource "azurerm_service_plan" "app_service_plan" {
  name                = var.app_serviceplan_name
  resource_group_name = var.resource_group_name
  location            = var.location

  os_type             = var.os_type
  sku_name            = var.sku_name

  tags = merge(var.tags, var.policy_tags)
}

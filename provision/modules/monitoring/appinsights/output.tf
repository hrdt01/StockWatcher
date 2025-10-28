output "id" {
  value = azurerm_application_insights.app_insights.id
}

output "connectionString" {
  value = azurerm_application_insights.app_insights.connection_string
}

output "instrumentationKey" {
  value = azurerm_application_insights.app_insights.instrumentation_key
}
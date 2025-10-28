output "id" {
  value = azurerm_linux_function_app.azure_function.identity.0.principal_id
}

output "appfunction_service_url" {
  value = azurerm_linux_function_app.azure_function.default_hostname
}
output "principal_id" {
  value = azurerm_linux_web_app.app_service_linux.identity.0.principal_id
}

output "app_service_url" {
  value = azurerm_linux_web_app.app_service_linux.default_hostname
}
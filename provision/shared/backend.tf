terraform {
  backend "azurerm" {
    storage_account_name = "__TerraformBackendAzureRmStorageAccountName__"
    container_name       = "__TerraformBackendAzureRmContainerName__"
    access_key           = "__TerraformBackendAzureRmAccessKey__"
    features {}
  }
}

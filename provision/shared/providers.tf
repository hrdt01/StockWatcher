terraform {
  required_version = ">=1.6.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 4.16.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = ">= 3.1.0"
    }    
    # azuredevops = {
    #   source  = "microsoft/azuredevops"
    #   version = ">= 1.5.0"
    # }
    random = {
      source  = "hashicorp/random"
      version = ">= 3.6.3"
    }
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy = true      
    }
    resource_group {
      prevent_deletion_if_contains_resources = false
    }    
  }
}

data "azurerm_client_config" "current" {}

provider "azuread" {
  client_id = data.azurerm_client_config.current.client_id
  tenant_id = data.azurerm_client_config.current.tenant_id  
}

data "azuread_application_published_app_ids" "well_known" {}
data "azurerm_subscription" "current" {}

# provider "azuredevops" {
#   org_service_url = "https://dev.azure.com/hactema"
#   # personal_access_token = 
# }
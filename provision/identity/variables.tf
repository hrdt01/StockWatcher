
variable "stocktracker_resource_group_name" {
  type        = string
  description = "Name of the Resource Group to store StockTracker artifacts"
}

variable "stocktracker_storage_account_name"{
  type        = string
  description = "Name of the Storage Account to store StockTracker artifacts"
}

variable "stocktrackeridentity_app_serviceplan_name"{
  type        = string
  description = "Name of the App Service Plan for Identity API"
}

variable "stocktrackeridentity_app_serviceplan_sku_name"{
  type        = string
  default     = "F1"
  description = "Name of the App Service Plan SKU for Identity API"
}

variable "stocktrackeridentity_appservice_name"{
  type        = string
  default     = "F1"
  description = "Name of the Azure AppService for Identity API"
}

variable "location" {
  type    = string
  default = "westeurope"
}

variable "policy_tags" {
  type        = map(string)
  description = "Tags which are mandatory on the Azure tenant."
  default = {
    "CostCenter"      = "Mi bolsillo",    
    "Criticality"     = "Medium",
    "ApplicationName" = "StockTracker"
  }
}

variable "general_tags" {
  type = map(any)

  default = {
    Scope     = "GENERAL"
  }
}

variable "terraform_tags" {
  type = map(any)

  default = {
      Component = "COMPUTE"
      Scope     = "TERRAFORM"
  }
}

variable "app_insights_rg_name" {
  type        = string
  description = "The Application Insights Resource group name"
  nullable    = false
}

variable "app_insights_name" {
  type        = string
  description = "The Application Insights name"
  nullable    = false
}
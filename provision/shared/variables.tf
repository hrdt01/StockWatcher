# variable "serviceconnection_spn" {
#   type        = string
#   default     = "hactema-StockTracker-44416764-95de-4342-9c7f-a28bd4f1c56b"
#   description = "Name of the service principal managing the AZDO service connection"
# }

# variable "azdo_target_project" {
#   type        = string
#   default     = "hactema-StockTracker-44416764-95de-4342-9c7f-a28bd4f1c56b"
#   description = "Name of the AZDO project"
# }

# variable "azdo_variable_group_name"{
#   type        = string
#   default     = "Terraform"
#   description = "Name of the AZDO Variable Group for Terraform pipeline variables"
# }

variable "stocktracker_resource_group_name" {
  type        = string
  default     = "rg-stocktracker"
  description = "Name of the Resource Group to store StockTracker artifacts"
}
variable "stocktracker_storage_account_name"{
  type        = string
  default     = "ststocktracker"
  description = "Name of the Storage Account to store StockTracker artifacts"
}

variable "stocktracker_endofday_tablename"{
  type        = string
  description = "Name of the Storage Account table to store EndOfDay information"
}

variable "stocktracker_kpimessagebroker_queuename"{
  type        = string
  description = "Name of the Storage Account queue to store request to process Kpis"
}

variable "stocktracker_cleanupmessagebroker_queuename"{
  type        = string
  description = "Name of the Storage Account queue to store request to process cleanup"
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
      Component = "STORAGE"
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

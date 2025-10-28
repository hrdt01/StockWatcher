variable "resource_group_name" {
  type        = string
  description = "RG name"
}

variable "location" {
  type        = string
  description = "Location"
}

variable "azure_function_name" {
  type        = string
  description = "app function name"
}

variable "service_plan_id" {
  type        = string
  description = "Id of service plan"
}

variable "storage_account_name" {
  type        = string
  description = "storage_account name"
}

variable "storage_account_accesskey" {
  type        = string
  description = "storage_account access key"
}

variable "appsetting_storage_connectionstring" {
  type = string
  description = "Connection of storage account"
}

variable "appsetting_queue_storage_connectionstring" {
  type = string
  description = "Connection of storage account"
}

variable "appsetting_function_cronexpression" {
  type = string
  description = "Function cron expression"
}

variable "appsetting_function_cleanupprocess_enabled" {
  type = string
  description = "Function Cleanup Prcess enabled flag"
}

variable "appsetting_appinsights_connstr" {
  type = string
  description = "Application Insights connection string"
  default = ""
}

variable "tags" {
  type        = map(string)
  description = "Tags for infrastructure resources."
  default     = {}
}

variable "policy_tags" {
  type        = map(string)
  description = "Tags which are mandatory on the Azure tenant."
  nullable    = false
}

variable "cors_allowed_origins" {
  type        = list(string)
  description = "Collection of origins to setup in CORS"
  nullable    = false
  default     = [ ]
}
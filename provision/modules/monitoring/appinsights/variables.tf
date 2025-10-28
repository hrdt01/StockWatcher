variable "app_insights_name" {
  type        = string
  description = "The Application Insights name"
  nullable    = false
}

variable "resource_group_name" {
  type        = string
  description = "The Resource Group name"
  nullable    = false
}

variable "location" {
  type        = string
  description = "The location"
  nullable    = false
}

variable "application_type" {
  type        = string
  description = "The application type"
  default     = "web"
  nullable    = false
}

variable "internet_ingestion_enabled" {
  type        = bool
  description = "Whether ingestion from the public internet is enabled for this insights service."
  default     = true
  nullable    = false
}

variable "internet_query_enabled" {
  type        = bool
  description = "Whether querying from the public internet is enabled for this insights service."
  default     = false
  nullable    = false
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
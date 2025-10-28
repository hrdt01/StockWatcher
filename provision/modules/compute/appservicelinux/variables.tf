variable "resource_group_name" {
  type        = string
  description = "Azure Resource Group name"
}

variable "app_service_name" {
  type        = string
  description = "App service resource name"
}

variable "location" {
  type        = string
  description = "Location"
}

variable "service_plan_id" {
  type        = string
  description = "Id of service plan"
}

variable "minimum_tls_version" {
  type        = string
  description = "Tls Version"
  default     = "1.2"
}

variable "always_on" {
  type        = bool
  default     = false
  description = "If this web app is always on"
}

variable "use_32_bit_worker" {
  type        = bool
  default     = true
  description = "(Optional) Should the Linux Web App use a 32-bit worker. Defaults to true."
}

variable "websockets_enabled" {
  type        = bool
  default     = false
  description = "(Optional) Should the WebSockets enabled for connections using WebSockets API protocol. Defaults to false."
}

variable "application_stack" {
  type = object({
    docker_image_name   = string
    docker_registry_url = string
  })
  default = {
    docker_image_name   = ""
    docker_registry_url = ""
  }
  description = "The Application Stack configuration for the Linux Web App."
}

variable "log_file_system" {
  type = object({
    retention_in_days = number
    retention_in_mb   = number
  })
  description = "Log file system configuration."
  default = {
    retention_in_days = 30
    retention_in_mb   = 50
  }
}

variable "app_settings" {
  type        = map(string)
  default     = {}
  description = "(Optional) A map of key-value pairs of App Settings."
}

variable "cors_allowed_origins" {
  type        = list(string)
  description = "Collection of origins to setup in CORS"
  nullable    = false
  default     = [ ]
}

variable "client_affinity_enabled" {
  type        = bool
  description = "Should the App Service send session affinity cookies, which route client requests in the same session to the same instance. Default false"
  default     = false
}

variable "tags" {
  type        = map(string)
  description = "Tags for infrastructure resources."
  default     = {}
}

variable "policy_tags" {
  type        = map(string)
  description = "Tags which are mandatory on the Azure Ypsomed tenant."
  nullable    = false
}

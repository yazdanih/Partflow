variable "project_name" {
  description = "Short name used in Azure resource names."
  type        = string
  default     = "partflow"

  validation {
    condition     = can(regex("^[a-z0-9-]{3,20}$", var.project_name))
    error_message = "project_name must contain 3-20 lowercase letters, numbers, or hyphens."
  }
}

variable "environment" {
  description = "Environment name used in resource names and tags."
  type        = string
  default     = "dev"

  validation {
    condition     = can(regex("^[a-z0-9]{2,10}$", var.environment))
    error_message = "environment must contain 2-10 lowercase letters or numbers."
  }
}

variable "location" {
  description = "Azure region for PartFlow resources."
  type        = string
  default     = "swedencentral"
}

variable "aks_node_count" {
  description = "Number of nodes in the AKS system pool."
  type        = number
  default     = 1

  validation {
    condition     = var.aks_node_count >= 1 && var.aks_node_count <= 3
    error_message = "aks_node_count must be between 1 and 3 for this portfolio environment."
  }
}

variable "aks_node_vm_size" {
  description = "VM size for the AKS system node pool."
  type        = string
  default     = "Standard_B2s"
}

variable "tags" {
  description = "Additional tags applied to Azure resources."
  type        = map(string)
  default     = {}
}

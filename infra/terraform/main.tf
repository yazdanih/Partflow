resource "random_string" "acr_suffix" {
  length  = 6
  lower   = true
  numeric = true
  special = false
  upper   = false
}

locals {
  resource_prefix = "${var.project_name}-${var.environment}"
  acr_prefix      = replace(lower(var.project_name), "/[^0-9a-z]/", "")

  common_tags = merge(var.tags, {
    Environment = var.environment
    ManagedBy   = "Terraform"
    Project     = "PartFlow"
  })
}

resource "azurerm_resource_group" "partflow" {
  name     = "rg-${local.resource_prefix}"
  location = var.location
  tags     = local.common_tags
}

resource "azurerm_container_registry" "partflow" {
  name                = "${local.acr_prefix}${var.environment}${random_string.acr_suffix.result}"
  resource_group_name = azurerm_resource_group.partflow.name
  location            = azurerm_resource_group.partflow.location
  sku                 = "Basic"
  admin_enabled       = false
  tags                = local.common_tags
}

resource "azurerm_kubernetes_cluster" "partflow" {
  name                = "aks-${local.resource_prefix}"
  location            = azurerm_resource_group.partflow.location
  resource_group_name = azurerm_resource_group.partflow.name
  dns_prefix          = local.resource_prefix
  sku_tier            = "Free"

  default_node_pool {
    name            = "system"
    node_count      = var.aks_node_count
    vm_size         = var.aks_node_vm_size
    os_disk_size_gb = 30
    type            = "VirtualMachineScaleSets"
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin    = "azure"
    load_balancer_sku = "standard"
  }

  role_based_access_control_enabled = true
  tags                              = local.common_tags
}

resource "azurerm_role_assignment" "aks_acr_pull" {
  scope                            = azurerm_container_registry.partflow.id
  role_definition_name             = "AcrPull"
  principal_id                     = azurerm_kubernetes_cluster.partflow.kubelet_identity[0].object_id
  skip_service_principal_aad_check = true
}

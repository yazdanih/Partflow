output "resource_group_name" {
  description = "Name of the PartFlow resource group."
  value       = azurerm_resource_group.partflow.name
}

output "container_registry_name" {
  description = "Globally unique Azure Container Registry name."
  value       = azurerm_container_registry.partflow.name
}

output "container_registry_login_server" {
  description = "Login server used when tagging PartFlow container images."
  value       = azurerm_container_registry.partflow.login_server
}

output "aks_cluster_name" {
  description = "Name of the PartFlow AKS cluster."
  value       = azurerm_kubernetes_cluster.partflow.name
}

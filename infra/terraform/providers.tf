provider "azurerm" {
  features {}

  # Keep plan read-only. Provider registration can be handled explicitly
  # before a future apply with the required Azure permissions.
  resource_provider_registrations = "none"
}

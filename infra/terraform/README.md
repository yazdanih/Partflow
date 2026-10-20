# PartFlow Azure infrastructure

This Terraform configuration describes a minimal portfolio environment:

- one Azure Resource Group;
- one Basic Azure Container Registry;
- one Free-tier AKS control plane with a single system node;
- one `AcrPull` role assignment for the AKS kubelet identity.

## Plan only

Planning reads Azure metadata but does not create the described resources.
From this directory, authenticate and keep the subscription ID outside the
Terraform files:

```powershell
az login
$env:ARM_SUBSCRIPTION_ID = az account show --query id --output tsv
terraform init
terraform fmt -check
terraform validate
terraform plan
Remove-Item Env:ARM_SUBSCRIPTION_ID
```

Do not run `terraform apply` until the expected Azure costs, quotas, resource
names, region, and permissions have been reviewed explicitly.

## Cost warning

The AKS `Free` tier applies only to the control plane. An apply would still
create billable VM, disk, network, and Basic ACR resources. Stopping an AKS
cluster does not necessarily remove every associated charge.

## Deliberate limitations

- State is local for now; a remote Azure Storage backend can be introduced
  before team or CI usage.
- Automatic Azure resource-provider registration is disabled to keep planning
  free from registration side effects. Required providers must be registered
  explicitly before a future apply.
- Networking is intentionally minimal and should be hardened before production.
- The node pool is sized for learning, not production reliability.

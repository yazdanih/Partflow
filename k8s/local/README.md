# PartFlow on Docker Desktop Kubernetes

These manifests run PartFlow in the local `partflow` namespace. PostgreSQL
uses a persistent volume, while the API is exposed through a local
LoadBalancer service on port 8081.

## Resources

- `ConfigMap`: non-sensitive application and database settings.
- `Secret`: the PostgreSQL password, created from the ignored local `.env`.
- `PersistentVolumeClaim`: 1 GiB of persistent PostgreSQL storage.
- `Deployment`: one PostgreSQL pod and one API pod.
- `Service`: internal PostgreSQL DNS and local API access.

## Deploy

Enable Docker Desktop Kubernetes with the `kind` provisioner first. Then run
these commands from the repository root.

Build the API image:

```powershell
docker build -t partflow-api:k8s-local -f PartFlow.Api/Dockerfile .
```

Create the namespace and a Secret containing only the password from the
ignored `.env` file. The value is kept in memory briefly and is not printed:

```powershell
kubectl apply -f k8s/local/namespace.yaml
$secretValue = ((Get-Content .env | Where-Object { $_ -match '^POSTGRES_PASSWORD=' }) -split '=', 2)[1]
$secretYaml = kubectl create secret generic partflow-db-secret --namespace partflow --from-literal=POSTGRES_PASSWORD=$secretValue --dry-run=client -o yaml
$secretYaml | kubectl apply -f -
Remove-Variable secretValue, secretYaml
```

Apply the remaining resources:

```powershell
kubectl apply -k k8s/local
kubectl get all,pvc -n partflow
```

When the API service has an external address, open:

```text
http://localhost:8081/api/parts
```

## Safe cleanup

Deleting the namespace also deletes its workloads. Do not delete the
namespace or PVC if its PostgreSQL data must be preserved.

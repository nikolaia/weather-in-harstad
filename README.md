# En backend for hobbyprosjekter

## Run the project

```bash
dotnet restore
dotnet build
cd src/Weather.Web
dotnet watch run
```

## Infrastructure

Download the Azure CLI 2.0. Login to your Azure account with `az login`.

```bash
cd infrastructure
sh azure_webapp.sh > last_output.json
```
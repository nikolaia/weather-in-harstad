#/bin/bash

# Install Azure CLI 2.0: curl -L https://aka.ms/InstallAzureCli | bash
# Login: az login

# Variables
resourceGroupName="miniseminaret2017"
servicePlanName="AppServicePlan"
appName="ms17app"
location="westeurope"

# Create a Resource Group
az group create --name $resourceGroupName --location $location

# Create an App Service Plan
az appservice plan create --name $servicePlanName --resource-group $resourceGroupName --location $location --is-linux --sku S1

# Create a Web App
az webapp create --name $appName --plan $servicePlanName --resource-group $resourceGroupName --is-linux

# Create local-git source-control for the Web App
az webapp deployment source config-local-git --name $appName --resource-group $resourceGroupName

# Create a Postgres Server
az postgres server create -l $location -g $resourceGroupName -n miniseminaret2017 -u miniseminaret2017 -p "8be7AzZuty*R9zdQMuPX2jfs&%Y22Z"

# git remote add azure https://@MyUniqueApp.scm.azurewebsites.net/MyUniqueApp.git

# Configure Web App with a Custom Docker Container from Docker Hub
# az webapp config container set --docker-custom-image-name $dockerHubContainerPath --name $appName --resource-group myResourceGroup

# Clean up deployment
# az group delete --name $resourceGroupName
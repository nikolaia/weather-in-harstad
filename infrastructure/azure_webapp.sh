#/bin/bash

# > last_output.json

# Install Azure CLI 2.0: curl -L https://aka.ms/InstallAzureCli | bash
# Login: az login

# : ${DBPASS?"Need to set DBPASS"} # ex: 8be7AzZuty*R9zdQMuPX2jfs&%Y22Z

gitrepo=$(git config --get remote.origin.url)
resourceGroupName=miniseminaret$RANDOM
location=westeurope
webappname=ms17app$RANDOM

# Create a resource group.
az group create --location $location --name $resourceGroupName

# Create an App Service plan in STANDARD tier (minimum required by deployment slots).
az appservice plan create --name $webappname --resource-group $resourceGroupName --sku S1

# Create a web app.
az webapp create --name $webappname --resource-group $resourceGroupName --plan $webappname

 #Create a deployment slot with the name "staging".
az webapp deployment slot create --name $webappname --resource-group $resourceGroupName --slot staging

# Deploy sample code to "staging" slot from GitHub.
az webapp deployment source config --name $webappname --resource-group $resourceGroupName --slot staging --repo-url $gitrepo --branch master --manual-integration

# Create a Postgres Server
#az postgres server create -l $location -g $resourceGroupName -n $webappname -u $webappname -p $DBPASS

# Browse to the deployed web app on staging. Deployment may be in progress, so rerun this if necessary.
#az webapp browse --name $webappname --resource-group $resourceGroupName --slot staging

# Swap the verified/warmed up staging slot into production.
#az webapp deployment slot swap --name $webappname --resource-group $resourceGroupName --slot staging

# Browse to the production slot.
#az webapp browse --name $webappname --resource-group myResourceGroup

# Clean up deployment
#az group delete --name $resourceGroupName
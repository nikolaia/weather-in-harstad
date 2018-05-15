Param(
    [Parameter(Mandatory=$True,Position=1)]
    [string]$appName
  )

# VALIDATE

#az group deployment validate -g MyApp.Dev --template-file "arm/azuredeploy.json" --parameters @arm/parameters/dev.parameters.json --query 'properties.provisioningState'
#az group deployment validate -g MyApp.Test --template-file "arm/azuredeploy.json" --parameters @arm/parameters/test.parameters.json --query 'properties.provisioningState'
#az group deployment validate -g MyApp --template-file "arm/azuredeploy.json" --parameters @arm/parameters/production.parameters.json --query 'properties.provisioningState'

# TEMPLATE DEPLOY

# az group deployment create -g MyApp.Test --template-file "arm/azuredeploy.json" --parameters @arm/parameters/dev.parameters.json

# ZIP DEPLOY APP

$token = (ConvertFrom-Json -InputObject ([string](Invoke-Expression -Command:"az account get-access-token")))."accessToken"
$bearerToken = "Bearer $token"
$apiUrl = "https://$appName.scm.azurewebsites.net/api/zipdeploy"

$vstsWorkDir = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
$vstsReleaseDefName = $env:RELEASE_DEFINITIONNAME

$artifactDir = if ($vstsWorkDir -and $vstsReleaseDefName) { 
        "$($env:SYSTEM_DEFAULTWORKINGDIRECTORY)/$($env:RELEASE_DEFINITIONNAME)/drop"
    } else { 
        split-path -parent $MyInvocation.MyCommand.Definition
    }

$zip = Get-ChildItem -Path $artifactDir -Filter *.zip | Select-Object -First 1
$zipPath = "$artifactDir/$($zip.Name)"
Write-Host "Calling $apiUrl with $zipPath using token '$bearerToken'"
Invoke-RestMethod -Uri $apiUrl -Headers @{Authorization=$bearerToken} -Method POST -InFile $zipPath -ContentType "multipart/form-data"
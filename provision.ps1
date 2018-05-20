Param(
    [Parameter(Mandatory=$True,Position=1)]
    [string]$appName,

    [Parameter(Mandatory=$True,Position=2)]
    [string]$parameterSet
  )
  
$parameters = "infrastructure/parameters/$parameterSet.parameters.json"
if (-Not (Test-Path $parameters -PathType Leaf)) { 
    Write-Error "Unable to find parameter file for provided parameterSet '$appName' ($parameters)"
    exit 1 
}

if ((az group list --query "[?name=='$appName']") -eq "[]") {
    az group create --location westeurope --name $appName --tag CreatedBy="Nikolai Norman Andersen"
}

$provisioning = az group deployment create -g $appName --template-file 'infrastructure/azuredeploy.json' --parameters $parameters --parameters webAppName=$appName --query 'properties.provisioningState'
if ([string]$provisioning -ne '"Succeeded"') { Write-Error "ARM Template deployment failed"; exit 1 }

# Naive locating so it works both on the BuildServer and on the local machine:
$zip = Get-ChildItem -Path .\ -recurse -Filter *.zip | Select-Object -First 1 | % { $_.FullName } 
Write-Host "Deploying $zip"

# TODO: Add certificate
$deploy = az webapp deployment source config-zip -g $appName -n $appName --src $zip --query "status"
if ([string]$deploy -ne '4') { Write-Error "Deployment Failed"; exit 1 } # status 4 = success - status 3 = failed

Write-Host "Deploy Success"

$secrets = "infrastructure/populate-keyvault.azcli"
if (Test-Path $secrets -PathType Leaf) { 
    Write-Error "Found '$secrets'. Running"
    powershell -noexit "& $secrets"
}
exit 0
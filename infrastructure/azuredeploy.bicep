@description('Location for resource group')
param location string = resourceGroup().location

@description('The name of the web app that you wish to create.')
param webAppName string

@description('The minimum loglevel for Serilog to push')
param serilogLogLevel string

var resourceGroupLocation = location
var appNameShort = substring(webAppName, 0, 5)
var keyVaultName_var = '${appNameShort}-kv-${uniqueString(resourceGroup().id)}'
var sqlServerName_var = '${appNameShort}-sql-${uniqueString(resourceGroup().id)}'
var sqlDatabaseName = '${appNameShort}-db-${uniqueString(resourceGroup().id)}'
var sqlDatabaseAdminName = 'weatherinharstad'
var webappHostingPlanName = '${appNameShort}-asp-${uniqueString(resourceGroup().id)}'
var sqlserverAdminLogin = 'l${uniqueString(resourceGroup().id, '9A08DDB9-95A1-495F-9263-D89738ED4205')}'
var sqlserverAdminPassword = 'P${uniqueString(resourceGroup().id, '224F5A8B-51DB-46A3-A7C8-59B0DD584A41')}x!'

resource webAppHostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: webappHostingPlanName
  location: resourceGroupLocation
  sku: {
    name: 'S1'
    tier: 'Standard'
    size: '1'
    family: 'S'
    capacity: 1
  }
}

resource webAppName_resource 'Microsoft.Web/sites@2021-03-01' = {
  name: webAppName
  location: resourceGroupLocation
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: webAppHostingPlan.id
    siteConfig: {
      http20Enabled: true
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'Weather:StormSecretUri'
          value: ''
        }
        {
          name: 'Serilog:MinimumLevel:Default'
          value: serilogLogLevel
        }
      ]
      connectionStrings: [
        {
          name: 'WeatherSqlDb'
          type: 'SQLAzure'
          connectionString: 'Data Source=${sqlDatabaseName}.database.windows.net,1433;Database=${sqlDatabaseName}'
        }
      ]
    }
  }
  dependsOn: [
    sqlServerName
  ]
}

resource sqlServerName 'Microsoft.Sql/servers@2021-11-01-preview' = {
  name: sqlServerName_var
  location: resourceGroupLocation
  properties: {
    administratorLogin: sqlserverAdminLogin
    administratorLoginPassword: sqlserverAdminPassword
    publicNetworkAccess: 'Enabled'
  }
  tags: {
    displayName: 'SQL Server'
  }
  dependsOn: []
}

resource sqlServerName_sqlDatabaseName 'Microsoft.Sql/servers/databases@2021-11-01-preview' = {
  parent: sqlServerName
  name: sqlDatabaseName
  location: resourceGroupLocation
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824
  }
}

resource sqlServerName_activeDirectory 'Microsoft.Sql/servers/administrators@2021-11-01-preview' = {
  parent: sqlServerName
  name: 'ActiveDirectory'
  properties: {
    administratorType: 'ActiveDirectory'
    login: sqlDatabaseAdminName
    sid: webAppName_resource.identity.principalId
    tenantId: webAppName_resource.identity.tenantId
  }
}

resource keyVaultName 'Microsoft.KeyVault/vaults@2021-11-01-preview' = {
  name: keyVaultName_var
  location: resourceGroupLocation
  properties: {
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: webAppName_resource.identity.tenantId
    accessPolicies: [
      {
        objectId: webAppName_resource.identity.principalId
        tenantId: webAppName_resource.identity.tenantId
        permissions: {
          keys: [
            'all'
          ]
          secrets: [
            'all'
          ]
        }
      }
      {
        tenantId: webAppName_resource.identity.tenantId
        objectId: '035aeb7f-2300-4a6d-85ba-086b113316ef'
        permissions: {
          secrets: [
            'set'
            'list'
          ]
        }
      }
    ]
  }
}

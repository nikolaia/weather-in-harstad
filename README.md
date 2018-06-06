# Example app that shows the current weather in Harstad, Norway

This is an example app for demonstrating https://github.com/nikolaia/fake-webapp-deploy 

## Build, provision and deploy the project to an Azure WebApp

Install .NET Core 2.1 SDK (v2.1.300) or higher. This is the version that introduced global tools.

Install Azure CLI 2.0+, run `az login` and make sure you are on the correct subscription, then:

```powershell
dotnet tool install fake-cli -g --version 5.0.0*
fake run build.fsx
.\provision.ps1 -appName <WebAppNameYouWantTheAppToHave> -parameterSet test
```

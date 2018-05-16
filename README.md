# Example app that shows the current weather in Harstad, Norway

This is an example app for demonstrating https://github.com/nikolaia/fake-webapp-deploy 

## Build, provision and deploy the project to an Azure WebApp

Install Azure CLI 2.0+, run `az login` and make sure you are on the correct subscription, then:

```powershell
.\build.cmd Artifact
.\upload.cmd -appName <WebAppNameYouWantTheAppToHave> -parameterSet test
```
# Example app that shows the current weather in Harstad, Norway

This is an example app for demonstrating https://github.com/nikolaia/fake-webapp-deploy 

## Build, provision and deploy the project to an Azure WebApp

Install .NET Core 2.1 SDK (v2.1.300) or higher. This is the version that introduced global tools.

Install Azure CLI 2.0+, run `az login` and make sure you are on the correct subscription, then:

```powershell
dotnet tool install fake-cli -g --version 5.0.0*
fake run build.fsx
fake run provision.fsx appName=yourWebAppName parameterSet=test
```

If you want `provision.fsx` to take a specific zip-file (it will default to the first `appName*.zip` it can find recursively), provide it as an argument: `zip=./artifacts/Weather.1.0.0-beta4.zip`

The `deploy.fsx` deployment script is made to be run from inside of Kudu and is packed inside the _zip_ file during `build.fsx`. Kudu is triggered to launch `deploy.fsx` through the `.deployment`-file:

```ini
[config]
command = dotnet tool install fake-cli -g --version 5.0.0* & D:\local\UserProfile\.dotnet\tools\fake run deploy.fsx stageFolder=app/
```

## Global tools on KUDU

Global tools are not in PATH by default inside the kudu sandbox:

```powershell
PS D:\home\site\wwwroot> dotnet tool install fake-cli -g --version 5.0.0*
dotnet tool install fake-cli -g --version 5.0.0*
Tools directory 'D:\local\UserProfile\.dotnet\tools' is not currently on the PATH environment variable.

You can add the directory to the PATH by running the following command:

setx PATH "%PATH%;D:\local\UserProfile\.dotnet\tools"

You can invoke the tool using the following command: fake
Tool 'fake-cli' (version '5.0.0') was successfully installed.
```

We are not allowed to run the suggested command, but since the UserProfile folder is predictable we can simply run it from the hardcoded path inside of the `.deployment`-file.
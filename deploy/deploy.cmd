@echo off
cls
if not exist "tools\FAKE\tools\Fake.exe" "tools\nuget.exe" "install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion" "-Version" "4.63.2"

"tools\FAKE\tools\Fake.exe" deploy.fsx %* --nocache


dotnet restore %cd%\Tools.csproj
dotnet fake run build.fsx --target %*
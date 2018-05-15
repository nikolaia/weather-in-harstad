REM This is currently in preview:
REM dotnet tool install --g dotnet-fake
REM We should use that instead of the Tools.csproj
dotnet restore %cd%\Tools.csproj
dotnet fake run build.fsx --target %*
dotnet restore %cd%\Tools.csproj
dotnet fake run deploy.fsx --target %*
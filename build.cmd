dotnet restore %cd%\Tools.csproj
dotnet fake run build.fsx --target %*
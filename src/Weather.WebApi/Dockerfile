FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["src/Weather.Web/Weather.Web.csproj", "Weather.Web/"]
RUN dotnet restore "src/Weather.Web/Weather.Web.csproj"
COPY . .
WORKDIR "/src/Weather.Web"
RUN dotnet build "Weather.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Weather.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Weather.Web.dll"]

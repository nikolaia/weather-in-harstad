- title : En backend for hobbyprosjekter på rekordtid
- description : Finnes ikke noe kjipere enn når man har en god idé, men ikke helt vet hvor man skal starte. Man trenger jo å få satt opp noe i skyen, noe som kan lagre i en database og helst skal det logge ting slik at vi vet hva som skjer. Det må også være enkelt å putte ut en ny versjon av appen. Mye greier, men vi skal vell sannelig klare å komme oss fra “File -> New” til kjørende produkt på under halvtimen 💪.
- author : Nikolai Norman Andersen
- theme : night
- transition : default

***

# En backend for hobbyprosjekter
##  på rekordtid

***

## Ingenting gir verdi før det er i produksjon
### Kontinuerlige leveranser

---

## Nettskyen
### Public Cloud

---

## PaaS og SaaS
### Ikke finn opp hjulet på nytt

---

## Infrastructure-as-Code
### Bruk og kast

---

```bash
az group create --location $location --name $resourceGroupName

# Create an App Service plan in STANDARD tier
# (minimum required by deployment slots).
az appservice plan create --name $webappname \
    --resource-group $resourceGroupName --sku S1

# Create a web app.
az webapp create --name $webappname \
    --resource-group $resourceGroupName --plan $webappname

# Create a deployment slot with the name "staging".
az webapp deployment slot create --name $webappname \
    --resource-group $resourceGroupName --slot staging

# Deploy sample code to "staging" slot from GitHub.
az webapp deployment source config --name $webappname \
    --resource-group $resourceGroupName --slot staging \
    --repo-url $gitrepo --branch master --manual-integration
```

***

## .NET vs Java
### Hva er greia?

---

## .NET Core
### Moderne backendstack

---

```bash
$ dotnet new web --output src/Web
$ cd src/Web
$ dotnet run
Hosting environment: Development
Content root path: /Users/nikolaia/Development/hobbybackend/src/Web
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

---

```bash
Program.cs
Startup.cs
Web.csproj
bin
obj
wwwroot
```

---

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Folder Include="wwwroot\" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.0" />
  </ItemGroup>

</Project>
```

---

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build()
            .Run();
    }
}
```

---

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.Run(async (context) =>
        {
            await context.Response.WriteAsync("Hello World!");
        });
    }
}
```

***

### DEMO

***

# Oppsummert

* Cloud - Spesielt PaaS og SaaS
* Infrastructure-as-code
* .NET Core og ASP.NET Core

---

## github.com/nikolaia/hobbybackend
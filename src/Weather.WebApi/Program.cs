using System;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weather.WebApi.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();

// Since we integrate with external services we want to be able to cache the values for a short time in memory
services.AddMemoryCache();

// Add custom configuration
builder.Configuration
  .AddJsonFile("appsettings.json")
  .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json")
  .AddEnvironmentVariables()
  .Build();

// Setup up configuration both for usage in this Program.cs file and using the IOption pattern so it can be used in the app, see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
var appOptions = new WeatherOptions();
var appOptionsConfigurationSection = builder.Configuration.GetSection("Weather");
appOptionsConfigurationSection.Bind(appOptions);
services.Configure<WeatherOptions>(appOptionsConfigurationSection);

// To access one of our integrations we need a secret value they have provided us, which we store in an Azure KeyVault available to our application at runtime (Using Managed Identity)
services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddSecretClient(new Uri(appOptions.KeyVaultUri));
    clientBuilder.UseCredential(new DefaultAzureCredential());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapDefaultControllerRoute();

app.Run();
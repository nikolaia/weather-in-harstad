using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Weather.Models;
using Weather.WebApi.Options;

namespace Weather.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IMemoryCache _cache;
    private readonly IOptions<WeatherOptions> _options;
    private readonly IConfiguration _configuration;
    private readonly SecretClient _secretClient;

    public WeatherController(ILogger<WeatherController> logger, IMemoryCache memoryCache,
        IOptions<WeatherOptions> options, IConfiguration configuration, SecretClient secretClient)
    {
        _logger = logger;
        _options = options;
        _cache = memoryCache;
        _configuration = configuration;
        _secretClient = secretClient;
    }

    [HttpGet]
    public async Task<List<View.TemperatureViewModel>> Get()
    {
        _logger.LogInformation("Someone is attempting to fetch the weather for Harstad");

        const string logTemplate = "{provider} says {temperature} degrees. Putting in cache for 30 seconds!";

        if (!_cache.TryGetValue("yr", out Domain.Temperature yr))
        {
            yr = await Integrations.Yr.GetForecastHarstad();
            _logger.LogInformation(logTemplate, yr.Provider.ProviderString, yr.TempString);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));

            _cache.Set("yr", yr, cacheEntryOptions);
        }

        if (!_cache.TryGetValue("storm", out Domain.Temperature storm))
        {
            storm = await Integrations.Storm.GetForecast("Harstad", _secretClient, _options.Value.StormSecretName);
            _logger.LogInformation(logTemplate, storm.Provider.ProviderString, storm.TempString);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));

            _cache.Set("storm", storm, cacheEntryOptions);
        }

        var connectionString = _configuration.GetConnectionString("WeatherSqlDb");
        var sql = Integrations.Sql.GetForecastHarstad(connectionString);

        var temperatures = new List<View.TemperatureViewModel>
        {
            View.TemperatureViewModel.Create(yr),
            View.TemperatureViewModel.Create(storm),
            View.TemperatureViewModel.Create(sql)
        };

        _logger.LogInformation("Temperatures are @{Temperatures}", temperatures);

        return temperatures;
    }
}
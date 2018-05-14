using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Serilog;
using Weather.Models;

namespace Weather.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMemoryCache _cache;

        public IList<View.TempViewModel> Temperatures { get; private set; }

        public IndexModel(IMemoryCache memoryCache) {
            this._cache = memoryCache;
        }
        public async Task OnGet()
        {
            Log.Information("Someone is attempting to fetch the weather for Harstad");
            
            const string logtemplate = "{provider} says {temperature} degrees. Putting in cache for 30 seconds!";

            if (!_cache.TryGetValue("yr", out Domain.Temp yr))
            {
                yr = await Weather.Integrations.Yr.GetForecastHarstad();
                Log.Information(logtemplate, yr.Provider.ProviderString, yr.TempString);
        
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                _cache.Set("yr", yr, cacheEntryOptions); 
            } 
            
            if (!_cache.TryGetValue("storm", out Domain.Temp storm))
            {
                storm = await Weather.Integrations.Storm.GetForecast("Harstad");
                Log.Information(logtemplate, storm.Provider.ProviderString, storm.TempString);
        
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                _cache.Set("storm", storm, cacheEntryOptions); 
            } 

            Temperatures = new List<View.TempViewModel>
            {
                View.TempViewModel.Create(yr),
                View.TempViewModel.Create(storm),
                new View.TempViewModel("Grandpa", "Nice!")
            };
            
            Log.Information("Temperatures are @{Temperatures}", Temperatures);
        }
    }
}

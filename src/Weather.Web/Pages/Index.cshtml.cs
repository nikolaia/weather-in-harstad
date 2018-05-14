using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Serilog;
using Weather.Models;

namespace Weather.Web.Pages
{
    public class IndexModel : PageModel
    {
        public IList<View.TempViewModel> Temperatures { get; private set; }

        public async Task OnGet()
        {
            Log.Information("Someone is attempting to fetch the weather for Harstad");
            
            const string logtemplate = "{provider} says {temperature} degrees";
            
            var yr = await Weather.Integrations.Yr.GetForecastHarstad();
            Log.Information(logtemplate, yr.Provider.ProviderString, yr.TempString);
            
            var storm = await Weather.Integrations.Storm.GetForecast("Harstad");
            Log.Information(logtemplate, storm.Provider.ProviderString, storm.TempString);
            
            Temperatures = new List<View.TempViewModel>
            {
                View.TempViewModel.Create(yr),
                View.TempViewModel.Create(storm),
                new View.TempViewModel("Bestefar", "Nice!")
            };
            
            Log.Information("Temperatures are @{Temperatures}", Temperatures);
        }
    }
}

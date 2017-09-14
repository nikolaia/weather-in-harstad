using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Weather.Models;

namespace Weather.Web.Pages
{
    public class IndexModel : PageModel
    {

        public IList<View.TempViewModel> Temperatures { get; set; }

        public async Task OnGet()
        {
            Temperatures = new List<View.TempViewModel>();
            
            // Get weather from Yr
            var yr = await Weather.Integrations.Yr.GetForecastHarstad();
            Temperatures.Add(new View.TempViewModel(Domain.TempProvider.Yr, yr.TempString));
            
            // Get weather from Storm
            var storm = await Weather.Integrations.Storm.GetForecast("Harstad");
            Temperatures.Add(new View.TempViewModel(Domain.TempProvider.Storm, storm.TempString));
            
            // Set bestefars opinion
            Temperatures.Add(new View.TempViewModel(Domain.TempProvider.Bestefar, "Trist"));
        }
    }
}

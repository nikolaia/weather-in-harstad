using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Weather.Models;

namespace Weather.Web.Pages
{
    public class IndexModel : PageModel
    {

        public IList<View.TempViewModel> Temperatures { get; private set; }

        public async Task OnGet()
        {
            Temperatures = new List<View.TempViewModel>
            {
                View.TempViewModel.Create(await Weather.Integrations.Yr.GetForecastHarstad()),
                View.TempViewModel.Create(await Weather.Integrations.Storm.GetForecast("Harstad")),
                new View.TempViewModel("Bestefar", "Trist")
            };
        }
    }
}

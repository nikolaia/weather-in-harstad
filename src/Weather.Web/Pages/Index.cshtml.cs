using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Weather.Web.Pages
{
    public class IndexModel : PageModel
    {
        public string YrTemp { get; private set; }
        public string StormTemp { get; private set; }

        public async Task OnGet()
        {
            this.YrTemp = await Weather.Integrations.Yr.GetForecastGvarv();
            this.StormTemp = await Weather.Integrations.Storm.GetForecastGvarv("Harstad");
        }
    }
}

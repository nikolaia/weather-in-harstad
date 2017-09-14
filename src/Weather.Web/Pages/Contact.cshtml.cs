using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Weather.Web.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Nikolai's contact page.";
        }
    }
}

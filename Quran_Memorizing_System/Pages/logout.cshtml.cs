using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Quran_Memorizing_System.Pages
{
    public class Log_outModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("email");
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("name");
            return RedirectToPage("Login_Page");
        }
    }
}

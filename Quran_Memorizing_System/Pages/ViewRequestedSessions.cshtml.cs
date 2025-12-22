using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class ViewRequestedSessionsModel : PageModel
    {
        DB db;

        public DataTable Sessions { get; set; }

        public ViewRequestedSessionsModel(DB dB)
        {
            db = dB;
        }
        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("email");
            var role = HttpContext.Session.GetString("role");
            if (email == null)
            {
                TempData["ErrorMassage"] = "you must be signed in";
                return RedirectToPage("/home");
            }
            if (role == "Sheikh")
            {
                TempData["ErrorMassage"] = "you must be a participant";
                return RedirectToPage("/home");
            }

            Sessions = db.getsessionsrequestedbyuser(email);
            return Page();
        }
    }
}
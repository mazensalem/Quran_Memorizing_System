using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class ViewAllrequestsModel : PageModel
    {
        public DataTable Sessions { get; set; }

        DB db;

        public ViewAllrequestsModel(DB dB)
        {
            db = dB;
        }
        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("email");
            var role = HttpContext.Session.GetString("role");

            if (email == null)
            {
                TempData["ErrorMassage"] = "You must sign in";
                return RedirectToPage("/Home");
            }
            if (role == "Participant")
            {
                TempData["ErrorMassage"] = "you must be a shiehk";
                return RedirectToPage("/Home");
            }

            Sessions = db.getallsessions();
            return Page();
        }

        public void OnPostAccept(int id)
        {
            db.Acceptrequest(HttpContext.Session.GetString("email"), id);
            Sessions = db.getallsessions();
        }

        public void OnPostDeny(int id)
        {
            db.Denyrequest(id);
            Sessions = db.getallsessions();
        }
    }
}
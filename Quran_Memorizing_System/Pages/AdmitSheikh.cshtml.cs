using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class AdmitSheikhModel : PageModel
    {
        DB db;
        public AdmitSheikhModel(DB dB)
        {
            db = dB;
        }
        [BindProperty]
        public string SearchInput { get; set; } = "";

        public DataTable Sheikhs { get; set; } = new DataTable();
        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("role");
            var email = HttpContext.Session.GetString("email");
            if (email == null)
            {
                return RedirectToPage("/");
            }
            if (role == "Participant")
            {
                return RedirectToPage("/");
            }
            
            DataTable userdt = db.GetUser(email, role);
            if (!Convert.ToBoolean(userdt.Rows[0]["isverifed"]))
            {
                return RedirectToPage("/");
            }

            Sheikhs = db.getAllUnderReviewSheikhs();
            return Page();
        }

        public void OnPostSearch() {
            Sheikhs = db.getUnderReviewShikhs(SearchInput);
        }

        public void OnPostAdmit(string user)
        {
            if (db.admitSheikh(user))
            {
                TempData["SuccessMessage"] = "User Admited";
                Sheikhs = db.getUnderReviewShikhs(SearchInput);
            }
        }

        public void OnPostDeny(string user)
        {
            if (db.denySheikh(user))
            {
                TempData["SuccessMessage"] = "User Deleted";
                Sheikhs = db.getUnderReviewShikhs(SearchInput);
            }
        }
    }
}

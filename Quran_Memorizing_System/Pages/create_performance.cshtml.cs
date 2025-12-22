using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class create_performanceModel : PageModel
    {
        User user;
        DB db;

        public Dictionary<string, string> questions { get; set; }
        [BindProperty]
        public int grade { get; set; }

        [BindProperty]
        public int subid { get; set; }

        public create_performanceModel(DB dB)
        {
            user = new User();
            db = dB;
        }

        void setuser()
        {
            var role = HttpContext.Session.GetString("role");
            var email = HttpContext.Session.GetString("email");
            DataTable userdt = db.GetUser(email, role);

            user.UserName = Convert.ToString(userdt.Rows[0]["UserName"]);
            user.PhoneNumber = Convert.ToInt32(userdt.Rows[0]["Phone"]);
            user.Email = email;
            user.gender = Convert.ToString(userdt.Rows[0]["Gender"]);
            user.role = role;
            user.PhoneVisability = Convert.ToBoolean(userdt.Rows[0]["Phonevisability"]);
            user.DateOfBirth = Convert.ToDateTime(userdt.Rows[0]["DateofBirth"]).ToShortDateString();

            if (user.gender == "F")
            {
                user.gender = "Female";
            }
            else
            {
                user.gender = "Male";
            }

            if (user.role == "Participant")
            {
                user.isverified = false;
            }
            else
            {
                user.isverified = Convert.ToBoolean(userdt.Rows[0]["isverifed"]);
            }
        }
        public IActionResult OnGet(int id)
        {
            subid = id;
            if (HttpContext.Session.GetString("email") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in";
                return RedirectToPage("/Home");
            }
            setuser();
            if (!user.isverified)
            {
                TempData["ErrorMessage"] = "You aren't allowed in";
                return RedirectToPage("/Home");
            }

            questions = db.getsubmission(id, HttpContext.Session.GetString("email"));


            return Page();
            
        }


        public IActionResult OnPost()
        {
            db.addperformancereview(subid, grade, HttpContext.Session.GetString("email"));
            return RedirectToPage("/Home");
        }
    }
}

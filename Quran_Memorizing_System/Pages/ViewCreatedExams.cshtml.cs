using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;
using System.Diagnostics;

namespace Quran_Memorizing_System.Pages
{
    public class ViewCreatedExamsModel : PageModel
    {
        DB db;
        public User user { get; set; }
        public DataTable CreatedExams { get; set; }

        void setuser()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("email")))
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
        }
        public ViewCreatedExamsModel(DB dB)
        {
            db = dB;
            user = new User();
            CreatedExams = new DataTable();
        }

        public void OnGet()
        {
            setuser();
            CreatedExams = db.getCreatedExams(HttpContext.Session.GetString("email"));
        }

        public IActionResult OnPostDelete(int examid)
        {

            if (db.deleteExam(examid))
            {
                TempData["SuccessMessage"] = "You Deleted Sucessfuly";
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
            }

                setuser();
            CreatedExams = db.getCreatedExams(HttpContext.Session.GetString("email"));
            return Page();
        }
    }
}

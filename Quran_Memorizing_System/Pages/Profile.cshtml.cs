using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class ProfileModel : PageModel
    {
        DB db;
        [BindProperty]
        public string password { get; set; }

        public User user { get; set; }
        public ProfileModel(DB dB)
        {
            db = dB;
            user = new User();
        }
        public IActionResult OnGet()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return RedirectToPage("/Login_Page");
            }
            else
            {
                var role = HttpContext.Session.GetString("role");
                var email = HttpContext.Session.GetString("email");
                DataTable userdt = db.GetUser(email, role);
                user.role = role;
                user.Email = email;
                user.UserName = Convert.ToString(userdt.Rows[0]["UserName"]);
                
                if (role != "Admin")
                {
                    user.PhoneNumber = Convert.ToInt32(userdt.Rows[0]["Phone"]);
                    user.gender = Convert.ToString(userdt.Rows[0]["Gender"]);
                    user.PhoneVisability = Convert.ToBoolean(userdt.Rows[0]["Phonevisability"]);
                    user.DateOfBirth = Convert.ToDateTime(userdt.Rows[0]["DateofBirth"]).ToShortDateString();
                }

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
                }else if (user.role == "Admin")
                {
                    user.isverified = true;
                }
                else
                {
                    user.isverified = Convert.ToBoolean(userdt.Rows[0]["isverifed"]);
                }


                return Page();
            }
        }

        public IActionResult OnPostDelete()
        {
            var role = HttpContext.Session.GetString("role");
            var email = HttpContext.Session.GetString("email");
            DataTable usertable = db.FindUser(email, password, role);
            if (usertable.Rows.Count == 1)
            {
                if (db.DeleteUser(email, role))
                {
                    TempData["SuccessMessage"] = "Your account has been deleted";
                    return RedirectToPage("/logout");
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong";
                    return RedirectToPage("/Profile");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "You Entered the wrong password";
                return RedirectToPage("/Profile");
            }
        }

    }
}

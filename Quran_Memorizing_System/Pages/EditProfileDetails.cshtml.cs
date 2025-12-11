using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Quran_Memorizing_System.Pages
{

    public class EditProfileDetailsModel : PageModel
    {
        DB db;

        
        [BindProperty]
        public User user { get; set; }

        
        public EditProfileDetailsModel(DB dB)
        {
            db = dB;
            user = new User();
        }
        
        public void OnGet()
        {
            var email = HttpContext.Session.GetString("email");
            var role = HttpContext.Session.GetString("role");
            DataTable user_dt = db.GetUser(email, role);
            var bdate = Convert.ToDateTime(user_dt.Rows[0]["Dateofbirth"]);

            user.UserName = Convert.ToString(user_dt.Rows[0]["UserName"]);
            user.Email = Convert.ToString(user_dt.Rows[0]["Email"]);
            user.DateOfBirth = bdate.ToString("yyyy-MM-dd");
            user.gender = Convert.ToString(user_dt.Rows[0]["Gender"]);
            user.PhoneNumber = Convert.ToInt32(user_dt.Rows[0]["Phone"]);
            user.PhoneVisability = Convert.ToBoolean(user_dt.Rows[0]["Phonevisability"]);

            user.gender = ((user.gender == "M") ? "Male" : "Female");
            user.role = role;
        }



        public IActionResult OnPost()
        {
            ModelState.Remove("user.Password");
            ModelState.Remove("user.confirmPassword");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            db.EditUser(user.Email, user.role, user);
            return RedirectToPage("/Profile");
        }
    }
}

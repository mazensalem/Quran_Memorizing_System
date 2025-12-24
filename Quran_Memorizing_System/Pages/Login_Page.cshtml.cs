using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class Login_Page : PageModel
    {
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        // role is not provided by the user anymore. It will be determined from the database after authentication.
        DB db;

        public string msg { get; set; }
        public Login_Page(DB Db)
        {
            db = Db;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Try to authenticate and determine role from the database
            var authResult = db.AuthenticateUser(Email, Password);
            DataTable res = authResult.Item1;
            string detectedRole = authResult.Item2;
            if (res.Rows.Count == 1 && !string.IsNullOrEmpty(detectedRole))
            {
                HttpContext.Session.SetString("role", detectedRole);
                HttpContext.Session.SetString("email", Convert.ToString(res.Rows[0]["Email"]));
                HttpContext.Session.SetString("name", Convert.ToString(res.Rows[0]["UserName"]));

                TempData["SuccessMessage"] = "Log IN Successful.";
                return RedirectToPage("/Index");
            }
            else
            {
                bool foundemail = db.EmailExistsAny(Email);
                if (foundemail)
                {
                    ModelState.AddModelError("Password", "This password is not correct");
                }
                else
                {
                    ModelState.AddModelError("Email", "This email doesn't exist");
                }
            }
            return Page();
        }
    }
}

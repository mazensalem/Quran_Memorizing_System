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

        [BindProperty]
        [Required]
        [RegularExpression(@"^(Participant|Sheikh)$")]
        public string role { get; set; }
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
            DataTable res = db.FindUser(Email, Password, role);
            if (res.Rows.Count == 1)
            {
                HttpContext.Session.SetString("role", role);
                HttpContext.Session.SetString("email", Convert.ToString(res.Rows[0]["Email"]));
                HttpContext.Session.SetString("name", Convert.ToString(res.Rows[0]["UserName"]));

                TempData["SuccessMessage"] = "Log IN Successful.";
                return RedirectToPage("/Profile");
            }
            else
            {
                bool foundemail = db.EmailExists(Email, role);
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

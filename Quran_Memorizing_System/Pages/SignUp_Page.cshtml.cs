using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;

namespace Quran_Memorizing_System.Pages
{
    public class SignUp_Page : PageModel
    {
        [BindProperty]
        public User user { get; set; }
        
        private DB db;
        public SignUp_Page(DB DB)
        {
            db = DB;
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

            if (db.PhoneExists(user.PhoneNumber, user.role))
            {
                ModelState.AddModelError("user.PhoneNumber", "This Phone is already Taken");
                return Page();
            }
            if (db.EmailExists(user.Email, user.role)) {
                ModelState.AddModelError("user.Email", "This Email is already Taken");
                return Page();
            }

            db.AddUser(user);
            TempData["SuccessMessage"] = "Congratulations! Your account has been created successfully.";
            return RedirectToPage("/Login_Page");
        }
    }
}

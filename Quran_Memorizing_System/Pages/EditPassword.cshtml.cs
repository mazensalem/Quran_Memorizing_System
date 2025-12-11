using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class EditPasswordModel : PageModel
    {
        DB db;

        [BindProperty]
        [Required]
        public string oldpassword { get; set; }

        [BindProperty]
        [Required]
        [StringLength(100, MinimumLength = 4)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string newpassword { get; set; }

        [BindProperty]
        [Required]
        [Compare("newpassword", ErrorMessage = "Passwords do not match")]
        public string newpasswordconfirm { get; set; }


        public EditPasswordModel(DB dB)
        {
            db = dB;
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
            
            db.changepassword(HttpContext.Session.GetString("email"), HttpContext.Session.GetString("role"), oldpassword, newpassword);
            return RedirectToPage("/Profile");
        }
    }
}

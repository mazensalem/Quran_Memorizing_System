using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class MemorizationCircleModel : PageModel
    {
        public DataTable Members { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        public DataTable Posts { get; set; }

        DB db;
        [BindProperty]
        [Required]
        [EmailAddress]
        public string addmemberemail { get; set; }

        public string role { get; set; }


        public MemorizationCircleModel(DB dB)
        {
            db = dB;
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return RedirectToPage("/Home");
            }
            Posts = db.GetAnnouncments(Name);
            Members = db.getusersincircule(Name);

            role = HttpContext.Session.GetString("role");
            
            return Page();
        }

        public IActionResult OnPostAddmember()
        {
            
            if (string.IsNullOrEmpty(addmemberemail) || !db.EmailExists(addmemberemail, "Participant") || db.isincircle(addmemberemail, Name))
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please Enter an email";
                    return RedirectToPage("/Memorization_Circle", new { Name = Name });
                }
                else if (db.isincircle(addmemberemail, Name))
                {
                    TempData["ErrorMessage"] = "This email is already in the circule";
                    return RedirectToPage("/Memorization_Circle", new { Name = Name });
                } else
                {
                    TempData["ErrorMessage"] = "This email is not registered";
                    return RedirectToPage("/Memorization_Circle", new { Name = Name });
                }
            }

            if (db.addtocircule(addmemberemail, Name))
            {
                TempData["SuccessMessage"] = "You added Successfuly";
            }
            else
            {
                TempData["ErrorMessage"] = "Somthing went wrong";
            }

            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }

        public IActionResult OnPostRemoveuser(string email)
        {
            if (db.removefromcircle(email, Name))
            {
                TempData["SuccessMessage"] = "You removed one person Successfuly";
            }
            else
            {
                TempData["ErrorMessage"] = "Somthing went wrong";
            }
            return RedirectToPage("/Memorization_Circle", new { Name = Name});
        }
    }
}

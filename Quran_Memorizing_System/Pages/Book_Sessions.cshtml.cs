using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;

namespace Quran_Memorizing_System.Pages
{
    public class Book_SessionsModel : PageModel
    {
        DB db;
        [BindProperty]
        public string Date { get; set; }

        [BindProperty]
        public int? StartPage { get; set; }
        [BindProperty]
        public int? EndPage { get; set; }
        [BindProperty]
        public string SectionType { get; set; }

        [BindProperty]
        public string SuraText { get; set; }

        public Book_SessionsModel(DB dB)
        {
            db = dB;
        }
        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("email");
            var role = HttpContext.Session.GetString("role");
            if (email == null)
            {
                TempData["ErrorMassage"] = "you must be signed in";
                return RedirectToPage("/home");
            }
            if (role == "Sheikh")
            {
                TempData["ErrorMassage"] = "you must be a participant";
                return RedirectToPage("/home");
            }

            return Page();
        }


        public IActionResult OnPost()
        {
            // Validate based on selected section type
            if (string.IsNullOrEmpty(SectionType))
            {
                ModelState.AddModelError("SectionType", "Please select a section type.");
                return Page();
            }

            if (SectionType == "Page" || SectionType == "Hizb")
            {
                if (!StartPage.HasValue || !EndPage.HasValue)
                {
                    ModelState.AddModelError("StartPage", "Please enter start and end numbers.");
                    return Page();
                }

                if (EndPage.Value <= StartPage.Value)
                {
                    ModelState.AddModelError("EndPage", "End must be greater than start.");
                    return Page();
                }

                db.requestsession(HttpContext.Session.GetString("email"), Date, SectionType, StartPage.Value, EndPage.Value, null);
            }
            else if (SectionType == "Sura")
            {
                if (string.IsNullOrWhiteSpace(SuraText))
                {
                    ModelState.AddModelError("SuraText", "Please enter sura text.");
                    return Page();
                }

                db.requestsession(HttpContext.Session.GetString("email"), Date, SectionType, 0, 0, SuraText);
            }

            TempData["SucessMassage"] = "You sucessfully requested a session";
            return RedirectToPage("Home");
        }
    }
}

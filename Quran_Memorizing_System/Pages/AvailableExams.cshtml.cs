using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class AvailableExamsModel : PageModel
    {
        private readonly DB _db ;
        public DataTable AvailableExams { get; set; }

        public AvailableExamsModel(DB db)
        {
            _db = db;
            AvailableExams = new DataTable();
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return RedirectToPage("/Login_Page");
            }
            else
            {
                string email = HttpContext.Session.GetString("email");
                AvailableExams = _db.getAvailableExams(email);

                return Page();
            }
        }
    }
}

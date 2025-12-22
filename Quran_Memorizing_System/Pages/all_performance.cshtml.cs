using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class all_performanceModel : PageModel
    {
        DB db;
        public DataTable submission { get; set; }
        public List<string> Examtitles { get; set; }
        public all_performanceModel(DB dB)
        {
            db = dB;
            Examtitles = new List<string> { };
        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in";
                return RedirectToPage("/Home");
            }
            submission = db.getallperformancereview(HttpContext.Session.GetString("email"));

            foreach (DataRow sub in submission.Rows)
            {
                Examtitles.Add(db.getExamtitlefromsubid(Convert.ToInt32(sub["Exam_Submission_ID"])));
            }

            return Page();
        }
    }
}

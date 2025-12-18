using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Collections.Specialized;
using System.Data;
using System.Security.Cryptography;

namespace Quran_Memorizing_System.Pages
{
    public class ExamsModel : PageModel
    {
        DB db;
        public DataTable Exam { get; set; }
        
        [BindProperty]
        public List<Question> Questions { get; set; }
        [BindProperty]
        public int subid { get; set; }
        public ExamsModel(DB dB)
        {
            db = dB;
            Exam = new DataTable();
        }
        public IActionResult OnGet(int examid)
        {
            Exam = db.getExam(examid);
            int sub_id = db.startExam(examid, HttpContext.Session.GetString("email"));
            Questions = db.getQuestiosn(examid);
            subid = sub_id; 
            return Page();
        }

        public IActionResult OnPost()
        {
            if (db.submitExam(HttpContext.Session.GetString("email"), Questions, subid))
            {
                TempData["SuccessMessage"] = "You finished your exam";
            }
            else
            {
                TempData["ErrorMessage"] = "Something is wrong";
            }
            return RedirectToPage("/Home");
        }
    }
}

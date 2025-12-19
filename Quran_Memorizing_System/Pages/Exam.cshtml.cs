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

        public int lefttime { get; set; }

        public ExamsModel(DB dB)
        {
            db = dB;
            Exam = new DataTable();
        }
        public IActionResult OnGet(int examid)
        {
            Exam = db.getExam(examid);

            if (HttpContext.Session.GetString("role") == "Sheikh")
            {
                TempData["ErrorMessage"] = "You are not a praticipant";
                return RedirectToPage("/home");
            }

            if (Exam.Rows.Count == 0)
            {
                TempData["ErrorMessage"] = "This exam doesn't exists";
                return RedirectToPage("/home");
            }

            if ((int)Exam.Rows[0]["PublicAvailablity"] == 0 && !Exam.Rows[0].IsNull("Circle_ID"))
            {
                bool found = false;
                int cid = (int)Exam.Rows[0]["Circle_ID"];
                DataTable circules = db.getusercirules(HttpContext.Session.GetString("email"), HttpContext.Session.GetString("role"));
                foreach (DataRow circule in circules.Rows)
                {
                    if ((int)circule["ID"] == cid)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    TempData["ErrorMessage"] = "You can't take this exam";
                    return RedirectToPage("/Home");
                }
            }

            if (Exam.Rows[0].IsNull("starttime") || Exam.Rows[0].IsNull("endtime"))
            { 

            }
            else { 
                if (DateTime.Now < (DateTime)Exam.Rows[0]["starttime"] || DateTime.Now > (DateTime)Exam.Rows[0]["endtime"])
                {
                    TempData["ErrorMessage"] = "This exam is sechuled for another time";
                    return RedirectToPage("/home");
                }
            }


            int sub_id = db.startExam(examid, HttpContext.Session.GetString("email"));

            lefttime = (int)Exam.Rows[0]["examduration"] - db.getavaliabletimeexam(sub_id);

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

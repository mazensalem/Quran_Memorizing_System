using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class GradeExamsModel : PageModel
    {
        [BindProperty (SupportsGet = true)]
        public int ExamID { get; set; }
        public DataTable Submissions { get; set; }

        DB db;
        public GradeExamsModel(DB dB)
        {
            db = dB;
        }

        public void OnGet()
        {
            Submissions = db.GetExamsubmission(ExamID);
        }
    }
}

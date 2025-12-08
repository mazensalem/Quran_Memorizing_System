using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;

namespace Quran_Memorizing_System.Pages
{
    public class question
    {
        public string Title;
        public string Prompt;
        public bool type;
        public List<string> Options;
    }
    public class ExamsModel : PageModel
    {
        public List<question> questions { get; set; }
        public void OnGet()
        {
            question q1 = new question();
            q1.Options = new List<string> { "Op1", "Op2" };
            q1.Title = "Q1";
            q1.type = false;

            question q2 = new question();
            q2.Title = "Q2";
            q2.type = true;
            q2.Options = null;

            questions = new List<question> { 
                q1,q2
            };
        }
    }
}
